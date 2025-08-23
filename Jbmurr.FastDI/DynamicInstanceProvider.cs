using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Jbmurr.FastDI
{
    internal class DynamicInstanceProvider : IInstanceProvider
    {
        private static readonly MethodInfo GetServiceGenericMethod =
            typeof(ServiceProvider)
                .GetMethods()
                .First(method => method.Name == nameof(ServiceProvider.GetService) && method.IsGenericMethodDefinition);
        public Func<ServiceProvider, object> Get(Type type)
        {
            var constructor = type.GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault()
                ?? throw new InvalidOperationException($"No public constructors found for {type}.");

            var serviceProviderParameter = Expression.Parameter(typeof(ServiceProvider), nameof(ServiceProvider));

            var serviceProviderMethodCall = constructor
                .GetParameters()
                .Select(x => Expression.Call(serviceProviderParameter, GetServiceGenericMethod.MakeGenericMethod(x.ParameterType)));

            var constructorInvocationExpression = Expression.New(constructor, serviceProviderMethodCall);
            var body = Expression.Convert(constructorInvocationExpression, typeof(object));

            var instanceFactoryExpression = Expression.Lambda<Func<ServiceProvider, object>>(body, serviceProviderParameter);
            return instanceFactoryExpression.Compile();
        }
    }
}
