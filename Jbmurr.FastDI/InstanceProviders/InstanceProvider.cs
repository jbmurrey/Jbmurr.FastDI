using Jbmurr.FastDI.ServicePlans;

namespace Jbmurr.FastDI.InstanceProviders
{
    internal class InstanceProvider : IInstanceProvider
    {
        public Func<ServiceProvider, object> Get(ServicePlan servicePlan)
        {
            return (serviceProvider) =>
            {
                return GetOrAddToCache(serviceProvider, servicePlan, CreateInstance);

                object CreateInstance()
                {
                    return servicePlan switch
                    {
                        FactoryPlan factoryPlan => servicePlan.Service.InstanceFactory!(serviceProvider),
                        ConstructorPlan constructorPlan => Get(serviceProvider, constructorPlan),
                        _ => null!,
                    };
                }
            };
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

        private static object GetOrAddToCache(ServiceProvider serviceProvider, ServicePlan servicePlan, Func<object> instanceFactory)
        {
            Dictionary<int, object>? cache = serviceProvider.FindServiceCache(servicePlan);

            if (cache == null)
            {
                return instanceFactory();
            }

            if (cache.TryGetValue(servicePlan.CacheKey, out var obj))
            {
                return obj;
            }

            object instance = instanceFactory();
            cache[servicePlan.CacheKey] = instance;
            return instance;
        }
    }
}
