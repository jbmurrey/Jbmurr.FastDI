using Jbmurr.FastDI.Abstractions;

namespace Jbmurr.FastDI
{
    internal class RegisteredServiceCache
    {
        private readonly RegisteredService[] _registeredServices;

        internal RegisteredServiceCache(ServiceCollection serviceCollection, IInstanceProvider instanceProvider)
        {
            IReadOnlyList<Service> services = [.. serviceCollection];
            _registeredServices = new RegisteredService[services.Count];

            PopulateCache(services, instanceProvider);
        }

        private void PopulateCache(IReadOnlyList<Service> services, IInstanceProvider instanceProvider)
        {
            int index = 0;
            foreach (var service in services)
            {
                _registeredServices[index] = new RegisteredService(service.Scope, service, instanceProvider.Get(service.ImplementationType));
                ServiceKeySetter.Set(service.ServiceType, index);
                index++;
            }
        }

        public RegisteredService GetRegisteredService<T>()
        {         
                return _registeredServices[ServiceKey<T>.Id];
        }
    }
}
