using Jbmurr.FastDI.Abstractions;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Jbmurr.FastDI
{
    internal sealed class RootServiceProvider : Abstractions.IServiceProvider
    {
        private HashSet<IDisposable> _disposibleInstances = [];
        private readonly ServiceProvider _rootServiceProvider;
        private readonly RegisteredServiceCache _serviceCache;
        private readonly ObjectCache _objectCache;
        private readonly ObjectCache _scopedCache;

        internal RootServiceProvider(ServiceCollection serviceCollection, IInstanceProvider instanceProvider)
        {
            IReadOnlyList<Service> services = [.. serviceCollection];

            _serviceCache = new RegisteredServiceCache(services, instanceProvider);
            _scopedCache = new ObjectCache(serviceCollection.GetScopedTypes());
            _objectCache = new ObjectCache(GetTypesToCache(serviceCollection));
            _rootServiceProvider = new ServiceProvider(this, _objectCache, isRoot: true);
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
            _scopedCache.Clear();
            return new ServiceProvider(this, _scopedCache, isRoot: false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetService<T>() where T : class
        {
            var registeredService = _serviceCache.GetRegisteredService<T>();

            return registeredService.Scope switch
            {
                Scope.Singleton or Scope.Scoped => _objectCache.GetOrAdd<T>(() => registeredService.InstanceFactory(_rootServiceProvider)),
                Scope.Transient => (T)registeredService.InstanceFactory(_rootServiceProvider),
                _ => throw new Exception($"Scope for type {typeof(T)} not found."),
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal T GetService<T>(ServiceProvider serviceProvider)
        {
            var registeredService = _serviceCache.GetRegisteredService<T>();


            return registeredService.Scope switch
            {
                Scope.Singleton => _objectCache.GetOrAdd<T>(() => registeredService.InstanceFactory(serviceProvider)),
                Scope.Scoped => serviceProvider.ObjectCache.GetOrAdd<T>(() => (T)registeredService.InstanceFactory(_rootServiceProvider)),
                Scope.Transient => (T)registeredService.InstanceFactory(_rootServiceProvider),
                _ => throw new Exception($"Scope for type {typeof(T)} not found."),
            };
        }

        public void Dispose()
        {
            if (_disposibleInstances is null)
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