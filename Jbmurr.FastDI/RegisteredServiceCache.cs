using Jbmurr.FastDI.Abstractions;

namespace Jbmurr.FastDI
{
    internal class RegisteredServiceCache
    {
        internal readonly RegisteredService[] _registeredServices;
 
        internal RegisteredServiceCache(IReadOnlyList<Service> services, IInstanceProvider instanceProvider)
        {
            _registeredServices = new RegisteredService[services.Count];

            PopulateCache(services, instanceProvider);
        }

        private void PopulateCache(IReadOnlyList<Service> services, IInstanceProvider instanceProvider)
        {
            int i = 0;
            foreach (var service in services.Where(x => x.Scope == Scope.Singleton))
            {
                Func<ServiceProvider, object> instanceFactory;
                if (service.InstanceFactory != null)
                {
                    instanceFactory = service.InstanceFactory;
                }
                else
                {
                    instanceFactory = instanceProvider.Get(service.ImplementationType);
                }

                ServiceKey.Set(service.ServiceType, i);
                _registeredServices[i] = new RegisteredService(service.Scope, service, instanceFactory, i);
                i++;
            }

            int j = 0;
            foreach (var service in services.Where(x => x.Scope == Scope.Scoped))
            {
                Func<ServiceProvider, object> instanceFactory;
                if (service.InstanceFactory != null)
                {
                    instanceFactory = service.InstanceFactory;
                }
                else
                {
                    instanceFactory = instanceProvider.Get(service.ImplementationType);
                }
                ServiceKey.Set(service.ServiceType, i);         
                _registeredServices[i++] = new RegisteredService(service.Scope, service, instanceFactory, j);
                j++;
            }

      
            foreach (var service in services.Where(x => x.Scope == Scope.Transient))
            {
                Func<ServiceProvider, object> instanceFactory;
                if (service.InstanceFactory != null)
                {
                    instanceFactory = service.InstanceFactory;
                }
                else
                {
                    instanceFactory = instanceProvider.Get(service.ImplementationType);
                }
                
                ServiceKey.Set(service.ServiceType, i);
                _registeredServices[i++] = new RegisteredService(service.Scope, service, instanceFactory, 0);
            }


        }

        internal RegisteredService GetRegisteredService<T>()
        {
            return _registeredServices[5];
        }
    }
}
