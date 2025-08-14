using Jbmurr.FastDI.Abstractions;

namespace Jbmurr.FastDI
{
    internal class RegisteredServiceCache
    {
        private readonly RegisteredService[] _registeredServices;
        private readonly KeyStore _keyStore;
        internal RegisteredServiceCache(IReadOnlyList<Service> services, IInstanceProvider instanceProvider)
        {
            _registeredServices = new RegisteredService[services.Count];
            _keyStore= new KeyStoreBuilder()
                .PopulateKeys(services.Select(x => x.ServiceType))
                .Build();
            PopulateCache(services, instanceProvider);
        }

        private void PopulateCache(IReadOnlyList<Service> services, IInstanceProvider instanceProvider)
        {
            Func<ServiceProvider, object> instanceFactory;

            foreach (var service in services)
            {
                if (service.InstanceFactory != null)
                {
                    instanceFactory = service.InstanceFactory;
                }
                else
                {
                    instanceFactory = instanceProvider.Get(service.ImplementationType);
                }

                var slot = _keyStore.Slot(service.ServiceType);
                _registeredServices[slot] = new RegisteredService(service.Scope, service, instanceFactory,slot);
            }
        }

        internal RegisteredService GetRegisteredService<T>()
        {
            int slot = _keyStore.Slot<T>();
            return _registeredServices[slot];
        }
    }
}
