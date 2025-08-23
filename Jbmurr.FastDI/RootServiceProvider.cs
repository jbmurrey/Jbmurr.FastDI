using Jbmurr.FastDI.Abstractions;

namespace Jbmurr.FastDI
{
    public sealed class RootServiceProvider : Abstractions.IServiceProvider
    {
        private readonly HashSet<IDisposable> _disposibleInstances = [];

        private readonly ServiceProvider _rootServiceProvider;
        private readonly RegisteredServiceCache _serviceCache;
        private readonly RegisteredService[] _registeredServices;
        private readonly object[] ObjectCache;
        private readonly int _scopedCount;


        internal RootServiceProvider(ServiceCollection serviceCollection, IInstanceProvider instanceProvider)
        {

            IReadOnlyList<Service> services = [.. serviceCollection];
            ObjectCache = new object[services.Count];
            _serviceCache = new RegisteredServiceCache(services, instanceProvider);
            _registeredServices = _serviceCache._registeredServices;
            _rootServiceProvider = new ServiceProvider(this, _scopedCount, true);
            _scopedCount = services.Where(x => x.Scope == Scope.Scoped).Count();
        }

        public Abstractions.IServiceProvider CreateScope()
        {
            return new ServiceProvider(this, _scopedCount);
        }

        public T GetService<T>() where T : class => GetService<T>(_rootServiceProvider);
    
        public object GetService(Type serviceType) => GetService(_rootServiceProvider, serviceType);

        internal T GetService<T>(ServiceProvider serviceProvider) where T : class
        {
            var registeredService = _registeredServices[ServiceKey<T>.Id];

            DependencyGuard<T>.Enter();
            T instance = GetInstance<T>(serviceProvider, registeredService);
            DependencyGuard<T>.Exit();

            return instance;
        }

        internal object GetService(ServiceProvider serviceProvider, Type serviceType)
        {
            return null;
        }

        private static T GetInstance<T>(ServiceProvider serviceProvider, RegisteredService registeredService) where T : class
        {
            if (TryGetCache(serviceProvider, registeredService.Scope, out object[]? cache))
            {
                ref var instance = ref cache![registeredService.CacheKey!.Value];
                instance ??= registeredService.InstanceFactory(serviceProvider);

                return (T)instance!;
            }

            return (T)registeredService.InstanceFactory(serviceProvider);
        }

        private static bool TryGetCache(ServiceProvider serviceProvider, Scope serviceScope, out object[]? cache)
        {
            cache = null;

            if ((serviceProvider.IsRoot && serviceScope == Scope.Scoped) || serviceScope == Scope.Singleton)
            {
                cache = serviceProvider.RootServiceProvider.ObjectCache;
            }
            else if (!serviceProvider.IsRoot && serviceScope == Scope.Scoped)
            {
                cache = serviceProvider.ObjectCache;
            }

            if (cache != null)
            {
                return true;
            }

            return false;
        }

        public void Dispose()
        {
            if (_disposibleInstances.Count == 0)
            {
                return;
            }

            foreach (var disposibleInstance in _disposibleInstances)
            {
                disposibleInstance.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}