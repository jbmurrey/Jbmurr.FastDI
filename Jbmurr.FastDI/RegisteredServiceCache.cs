using Jbmurr.FastDI.Abstractions;

namespace Jbmurr.FastDI
{
    internal class RegisteredServiceCache
    {
        private readonly RegisteredService[] _registeredServices;
        internal int[] ServiceKeyMapper { get; }
        internal int ScopedCount { get; private set; }
        internal int SingletonCount { get; private set; }

        internal RegisteredServiceCache(IReadOnlyList<Service> services, IInstanceProvider instanceProvider)
        {
            _registeredServices = new RegisteredService[services.Count];
            ServiceKeyMapper = new int[services.Count];
            PopulateCache(services, instanceProvider);
        }

        private void PopulateCache(IReadOnlyList<Service> services, IInstanceProvider instanceProvider)
        {
            int totalIndex = 0;
            int scopedIndex = 0;
            int singletonIndex = 0;
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

                _registeredServices[totalIndex] = new RegisteredService(service.Scope, service, instanceFactory);
                ServiceKeySetter.Set(service.ServiceType, totalIndex);

                switch (service.Scope)
                {
                    case Scope.Singleton:
                        ServiceKeyMapper[totalIndex] = singletonIndex++;
                        break;
                    case Scope.Scoped:
                        ServiceKeyMapper[totalIndex] = scopedIndex++;
                        break;
                    case Scope.Transient:
                        ServiceKeyMapper[totalIndex] = -1;
                        break;
                }

                totalIndex++;
            }

            ScopedCount = scopedIndex;
            SingletonCount = singletonIndex;
        }

        internal RegisteredService GetRegisteredService<T>()
        {
            return _registeredServices[ServiceKey<T>.Id];
        }
    }
}
