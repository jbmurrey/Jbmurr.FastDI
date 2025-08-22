using Jbmurr.FastDI.Abstractions;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Jbmurr.FastDI
{
    public sealed class RootServiceProvider : Abstractions.IServiceProvider
    {
        private readonly HashSet<IDisposable> _disposibleInstances = [];
        private readonly ServiceProvider _rootServiceProvider;
        private readonly RegisteredServiceCache _serviceCache;
        private readonly RegisteredService[] _registeredServices;
        private readonly RegisteredServiceCache _registeredServiceCache;
        private readonly object[] _cachedInstances;
        private readonly int _scopedCount;


        internal RootServiceProvider(ServiceCollection serviceCollection, IInstanceProvider instanceProvider)
        {

            IReadOnlyList<Service> services = [.. serviceCollection];
            _cachedInstances = new object[services.Count];
            _serviceCache = new RegisteredServiceCache(services, instanceProvider);
            _registeredServices = _serviceCache._registeredServices;
            _rootServiceProvider = new ServiceProvider(this, _scopedCount, true);
            _registeredServiceCache = new RegisteredServiceCache(services, instanceProvider);
            _scopedCount = services.Where(x => x.Scope == Scope.Scoped).Count();
        }

        private void PopulateCache(IReadOnlyList<Service> services, IInstanceProvider instanceProvider)
        {
        }

        private static Type[] GetTypesToCache(ServiceCollection serviceCollection)
        {
            return serviceCollection
                .Where(x => x.Scope == Scope.Singleton || x.Scope == Scope.Scoped)
                .Select(x => x.ServiceType)
                .ToArray();
        }

        public Abstractions.IServiceProvider CreateScope()
        {
            return new ServiceProvider(this, _scopedCount);
        }

        public T GetService<T>() where T : class
        {
            //var registeredService = _serviceCache.GetRegisteredService<T>();

            //return registeredService.Scope switch
            //{
            //    Scope.Singleton or Scope.Scoped => _objectCache.GetOrAdd<T>(() => registeredService.InstanceFactory(_rootServiceProvider)),
            //    Scope.Transient => (T)registeredService.InstanceFactory(_rootServiceProvider),
            //    _ => throw new Exception($"Scope for type {typeof(T)} not found."),
            //};

            return default;
        }

        internal T GetService<T>(ServiceProvider serviceProvider) where T : class
        {

            if (serviceProvider.IsRoot)
            {
                return GetService<T>();
            }

            var registeredService = _registeredServices[ServiceKey<T>.Id];

            T? instance = null;

            switch (registeredService.Scope)
            {
                case Scope.Singleton:
                    if (_cachedInstances[registeredService.CacheKey] is null)
                    {
                        instance = (T)registeredService.InstanceFactory(serviceProvider);
                        _cachedInstances[registeredService.CacheKey] = instance;
                    }
                    else
                    {
                        instance = (T)_cachedInstances[registeredService.CacheKey];
                    }
                    break;

                case Scope.Scoped:
                    if (serviceProvider.ObjectCache[registeredService.CacheKey] is null)
                    {
                        instance = (T)registeredService.InstanceFactory(serviceProvider);
                        serviceProvider.ObjectCache[registeredService.CacheKey] = instance;
                    }
                    else
                    {
                        instance = (T)_cachedInstances[registeredService.CacheKey];
                    }
                    break;

                case Scope.Transient:

                    instance = (T)registeredService.InstanceFactory(serviceProvider);
                    break;
            }

            return instance!;
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