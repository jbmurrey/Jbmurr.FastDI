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
            DependencyStack dependencyStack = new();

            return GetPlan(serviceType, dependencyStack);
        }


        internal ServicePlan GetPlan(Type serviceType, DependencyStack dependencyStack)
        {
            return _servicePlans.GetOrAdd(serviceType,
          (type) =>
          {
              var service = _services[type];
              dependencyStack.Push(service.ServiceType);
              var plan = GetPlan(service, _cacheKeyCounter++, dependencyStack);
              dependencyStack.Pop();

              return plan;
          });
        }


        private ServicePlan GetPlan(Service service, int cacheKey, DependencyStack dependencyStack)
        {
            if (service.InstanceFactory != null)
            {
                return new FactoryPlan(service, cacheKey);
            }

            var constructorInfo = service.ImplementationType.GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault() ?? throw new InvalidOperationException($"No public constructors found for {service.ServiceType}.");

            var parameters = constructorInfo.GetParameters();

            ServicePlan[] servicePlans = parameters.Select(x => GetPlan(x.ParameterType, dependencyStack)).ToArray();

            return new ConstructorPlan(service, constructorInfo, servicePlans, cacheKey);
        }
    }
}
