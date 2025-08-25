using Jbmurr.FastDI.ServicePlans;
using System.Collections.Concurrent;

namespace Jbmurr.FastDI.InstanceProviders
{
    internal class InstanceProvider : IInstanceProvider
    {
        public Func<ServiceProvider, object> Get(ServicePlan servicePlan)
        {
            return (serviceProvider) => serviceProvider.GetOrAddToCache(servicePlan, CreateInstance);

            object CreateInstance(ServiceProvider serviceProvider)
            {
                return servicePlan switch
                {
                    FactoryPlan factoryPlan => servicePlan.Service.InstanceFactory!(serviceProvider),
                    ConstructorPlan constructorPlan => Get(serviceProvider, constructorPlan),
                    _ => null!,
                };
            }
        }

        private object Get(ServiceProvider serviceProvider, ConstructorPlan constructorPlan)
        {
            object[] arguments = new object[constructorPlan.ConstructorParameters.Length];

            for (int index = 0; index < constructorPlan.ConstructorParameters.Length; index++)
            {
                arguments[index] = Get(constructorPlan.ConstructorParameters[index])(serviceProvider);
            }

            return constructorPlan.ConstructorInfo.Invoke(arguments);
        }
    }
}
