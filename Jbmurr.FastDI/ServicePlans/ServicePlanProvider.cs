using Jbmurr.FastDI.Abstractions;
using System.Collections.Concurrent;

namespace Jbmurr.FastDI.ServicePlans
{
    internal class ServicePlanProvider
    {
        private readonly ConcurrentDictionary<Type, ServicePlan> _servicePlans = [];
        private readonly IReadOnlyDictionary<Type, Service> _services;
        private int _cacheKeyCounter = 0;

        internal ServicePlanProvider(ServiceCollection services)
        {
            _services = services.ToDictionary(x => x.ServiceType, y => y);
        }

        internal ServicePlan GetPlan(Type serviceType)
        {
            return _servicePlans.GetOrAdd(serviceType,
            (type) =>
            {
                var service = _services[type];
                return GetPlan(service, _cacheKeyCounter++);
            });
        }


        private ServicePlan GetPlan(Service service, int cacheKey)
        {
            if (service.InstanceFactory != null)
            {
                return new FactoryPlan(service, cacheKey);
            }

            var constructorInfo = service.ImplementationType.GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault() ?? throw new InvalidOperationException($"No public constructors found for {service.ServiceType}.");

            var parameters = constructorInfo.GetParameters();

            ServicePlan[] servicePlans = parameters.Select(x => GetPlan(x.ParameterType)).ToArray();

            return new ConstructorPlan(service, constructorInfo, servicePlans, cacheKey);
        }
    }
}
