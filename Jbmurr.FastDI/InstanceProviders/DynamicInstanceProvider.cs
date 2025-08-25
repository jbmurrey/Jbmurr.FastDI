using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Jbmurr.FastDI.Abstractions;
using Jbmurr.FastDI.ServicePlans;

namespace Jbmurr.FastDI.InstanceProviders
{
    internal sealed class DynamicInstanceProvider : IInstanceProvider
    {
        private readonly ConcurrentDictionary<int, Func<ServiceProvider, object>> _cachedInstanceProviders = [];

        public Func<ServiceProvider, object> Get(ServicePlan servicePlan)
        {
            return _cachedInstanceProviders.GetOrAdd(servicePlan.Key, (_) => GetInstanceFactory(servicePlan));
        }

        private static Func<ServiceProvider, object> GetInstanceFactory(ServicePlan servicePlan)
        {
            var serviceProviderParameter = Expression.Parameter(typeof(ServiceProvider));
            return Expression.Lambda<Func<ServiceProvider, object>>(GetInstanceExpression(servicePlan, serviceProviderParameter), serviceProviderParameter).Compile();
        }

        private static Expression GetInstanceExpression(ServicePlan servicePlan, ParameterExpression serviceProviderParameter)
        {
            if (servicePlan.Service.Scope == Abstractions.Scope.Transient)
            {
                return GetInstanceFromServicePlan(servicePlan, serviceProviderParameter);
            }

            return GetInstanceWithCacheInline(servicePlan, serviceProviderParameter);
        }


        private static Expression GetInstanceWithCacheInline(ServicePlan servicePlan, ParameterExpression serviceProviderParameter)
        {
            if (servicePlan.Service.Scope == Scope.Transient)
            {
                var createInstanceExpression = GetInstanceFromServicePlan(servicePlan, serviceProviderParameter);
                return Expression.Convert(createInstanceExpression, servicePlan.Service.ServiceType);
            }

            var serviceProviderType = typeof(ServiceProvider);
            var rootServiceProviderProperty = serviceProviderType.GetProperty(nameof(ServiceProvider.RootServiceProvider),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var isRootProperty = serviceProviderType.GetProperty(nameof(ServiceProvider.IsRoot),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var scopeObjectCacheProperty = serviceProviderType.GetProperty(nameof(ServiceProvider.ObjectCache),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            var rootServiceProviderExpression = Expression.Property(serviceProviderParameter, rootServiceProviderProperty!);

            var rootServiceProviderType = typeof(RootServiceProvider);
            var rootObjectCacheProperty = rootServiceProviderType.GetProperty(nameof(RootServiceProvider.ObjectCache),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            var rootCacheExpression = Expression.Property(rootServiceProviderExpression, rootObjectCacheProperty!);
            var scopeCacheExpression = Expression.Property(serviceProviderParameter, scopeObjectCacheProperty!);
            var isRootExpression = Expression.Property(serviceProviderParameter, isRootProperty!);

            Expression dictionaryExpression =
                servicePlan.Service.Scope == Scope.Singleton
                    ? rootCacheExpression
                    : Expression.Condition(isRootExpression, rootCacheExpression, scopeCacheExpression);

            static FieldInfo GetLockInstanceField(Type t) =>
                typeof(Lock<>).MakeGenericType(t).GetField(nameof(Lock<object>.Instance),
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!;

            var rootLockExpression = Expression.Field(null, GetLockInstanceField(typeof(RootServiceProvider)));
            var scopeLockExpression = Expression.Field(null, GetLockInstanceField(typeof(ServiceProvider)));

            Expression lockObjectChoiceExpression =
                servicePlan.Service.Scope == Scope.Singleton
                    ? rootLockExpression
                    : Expression.Condition(isRootExpression, rootLockExpression, scopeLockExpression);

            var keyConstantExpression = Expression.Constant(servicePlan.Key, typeof(int));

            var tryGetValueMethod = typeof(Dictionary<int, object>).GetMethod(
                nameof(Dictionary<int, object>.TryGetValue),
                [typeof(int), typeof(object).MakeByRefType()])!;
            
            var indexerProperty = typeof(Dictionary<int, object>) .GetProperty("Item", [typeof(int)])!;

            var monitorEnterMethod = typeof(Monitor).GetMethod(
                nameof(Monitor.Enter),
                [typeof(object), typeof(bool).MakeByRefType()])!;
            var monitorExitMethod = typeof(Monitor).GetMethod(nameof(Monitor.Exit), new[] { typeof(object) })!;

            var valueVariable = Expression.Variable(typeof(object), "value");
            var lockObjectVariable = Expression.Variable(typeof(object), "lockObject");
            var lockTakenVariable = Expression.Variable(typeof(bool), "lockTaken");

            var tryGetValueCall = Expression.Call(dictionaryExpression, tryGetValueMethod, keyConstantExpression, valueVariable);

            var createInstanceExpressionInner = GetInstanceFromServicePlan(servicePlan, serviceProviderParameter);

            var assignCreate = Expression.Assign(valueVariable, createInstanceExpressionInner);

            var indexerExpression = Expression.Property(dictionaryExpression, indexerProperty, keyConstantExpression);
            var assignStore = Expression.Assign(indexerExpression, valueVariable);

            var missCondition = Expression.OrElse(
                Expression.IsFalse(tryGetValueCall),
                Expression.Equal(valueVariable, Expression.Constant(null, typeof(object))));

            var ifMissBlock = Expression.IfThen(missCondition, Expression.Block(assignCreate, assignStore));

            var enterLockCall = Expression.Call(monitorEnterMethod, lockObjectVariable, lockTakenVariable);

            var exitLockIfTaken = Expression.IfThen(
                lockTakenVariable,
                Expression.Call(monitorExitMethod, lockObjectVariable));

            var tryFinallyBlock = Expression.TryFinally(ifMissBlock, exitLockIfTaken);

            var bodyBlock = Expression.Block(
                new[] { valueVariable, lockObjectVariable, lockTakenVariable },
                Expression.Assign(lockObjectVariable, lockObjectChoiceExpression),
                Expression.Assign(lockTakenVariable, Expression.Constant(false)),
                enterLockCall,
                tryFinallyBlock,
                valueVariable);

            return Expression.Convert(bodyBlock, servicePlan.Service.ServiceType);
        }

        private static Expression GetInstanceFromServicePlan(ServicePlan servicePlan, ParameterExpression sp)
        {
            return servicePlan switch
            {
                FactoryPlan factoryPlan => Expression.Invoke(
                                         Expression.Constant(factoryPlan.InstanceFactory),
                                         Expression.Parameter(typeof(ServiceProvider))
                                     ),
                ConstructorPlan constructorPlan => GetInstanceNewExpression(constructorPlan, sp),
                _ => throw new NotSupportedException($"Service plan of type {servicePlan.GetType().Name} is not supported."),
            };
        }

        private static NewExpression GetInstanceNewExpression(ConstructorPlan constructorPlan, ParameterExpression sp)
        {
            List<Expression> Parameters = [];

            foreach (ServicePlan servicePlan in constructorPlan.ConstructorParameters)
            {
                Parameters.Add(GetInstanceExpression(servicePlan, sp));
            }

            return Expression.New(constructorPlan.ConstructorInfo, Parameters);
        }
    }
}
