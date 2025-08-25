using Jbmurr.FastDI.ServicePlans;
using System.Collections.Concurrent;

namespace Jbmurr.FastDI.InstanceProviders
{
    internal class InstanceProvider : IInstanceProvider
    {
        public Func<ServiceProvider, object> Get(ServicePlan servicePlan)
        {
            DependencyStack dependencyStack = new();

            return (serviceProvider) => serviceProvider.GetOrAddToCache(servicePlan, CreateInstance);

            object CreateInstance(ServiceProvider serviceProvider)
            {
                dependencyStack.Push(servicePlan.Key);

                return servicePlan switch
                {
                    FactoryPlan factoryPlan => servicePlan.Service.InstanceFactory!(serviceProvider),
                    ConstructorPlan constructorPlan => Get(serviceProvider, constructorPlan, dependencyStack),
                    _ => null!,
                };
            }
        }

        private object Get(ServiceProvider serviceProvider, ConstructorPlan constructorPlan, DependencyStack dependencyStack)
        {
            object[] arguments = new object[constructorPlan.ConstructorParameters.Length];

            for (int index = 0; index < constructorPlan.ConstructorParameters.Length; index++)
            {
                dependencyStack.Push(constructorPlan.Key);
                arguments[index] = Get(constructorPlan.ConstructorParameters[index])(serviceProvider);
                dependencyStack.Pop();
            }

            return constructorPlan.ConstructorInfo.Invoke(arguments);
        }

    }
}
