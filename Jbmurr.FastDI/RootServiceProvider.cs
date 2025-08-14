using Jbmurr.FastDI.Abstractions;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Jbmurr.FastDI
{
    internal sealed class RootServiceProvider : Abstractions.IServiceProvider
    {
        private readonly HashSet<IDisposable> _disposibleInstances = [];
        private readonly ServiceProvider _rootServiceProvider;
        private readonly RegisteredServiceCache _serviceCache;
        private readonly ObjectCache _objectCache;
        private readonly KeyStore _scopedKeyStore;

        internal RootServiceProvider(ServiceCollection serviceCollection, IInstanceProvider instanceProvider)
        {
            IReadOnlyList<Service> services = [.. serviceCollection];

            _serviceCache = new RegisteredServiceCache(services, instanceProvider);
            _objectCache = new ObjectCache(new KeyStoreBuilder()
                .PopulateKeys(GetTypesToCache(serviceCollection))
                .Build());

            _rootServiceProvider = new ServiceProvider(this, _objectCache, isRoot: true);
            _scopedKeyStore = new KeyStoreBuilder()
                .PopulateKeys(serviceCollection.GetScopedTypes())
                .Build();
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
            return new ServiceProvider(this, new ObjectCache(_scopedKeyStore), isRoot: false);
        }

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

        internal T GetService<T>(ServiceProvider serviceProvider)
        {
            var registeredService = _serviceCache.GetRegisteredService<T>();

            T instance;

            switch (registeredService.Scope)
            {
                case Scope.Singleton:
                    instance = (T)registeredService.InstanceFactory(_rootServiceProvider);
                    // instance = _objectCache.GetOrAdd<T>(() => registeredService.InstanceFactory(serviceProvider));

                    //if(instance is IDisposable singletonDisposable)
                    //{
                    //    _disposibleInstances.Add(singletonDisposable);
                    //}
                    break;

                case Scope.Scoped:
                    instance = (T)registeredService.InstanceFactory(_rootServiceProvider);
                    //  instance = serviceProvider.ObjectCache.GetOrAdd<T>(() => (T)registeredService.InstanceFactory(_rootServiceProvider));

                    //if(instance is IDisposable scopedDisposable)
                    //{

                    //    serviceProvider.DisposibleInstances.Add(scopedDisposable);
                    //}

                    break;
                case Scope.Transient:

                    instance = (T)registeredService.InstanceFactory(_rootServiceProvider);

                    //if (instance is IDisposable transientDisposable)
                    //{

                    //    serviceProvider.DisposibleInstances.Add(transientDisposable);
                    //}
                    break;

                default:
                    throw new Exception($"Scope for type {typeof(T)} not found.");
            }

            return instance;
        }

        public void Dispose()
        {
            foreach (var disposibleInstance in _disposibleInstances)
            {
                disposibleInstance.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}