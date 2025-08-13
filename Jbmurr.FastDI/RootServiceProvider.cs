using Jbmurr.FastDI.Abstractions;
using System;
using System.Runtime.CompilerServices;

namespace Jbmurr.FastDI
{
    internal sealed class RootServiceProvider : Abstractions.IServiceProvider
    {
        private HashSet<IDisposable> _disposibleInstances = [];
        private readonly ServiceProvider _rootServiceProvider;
        private readonly RegisteredServiceCache _serviceCache;
        private readonly object[] _cachedInstances;

        internal RootServiceProvider(ServiceCollection serviceCollection, IInstanceProvider instanceProvider)
        {

            IReadOnlyList<Service> services = [.. serviceCollection];

            _serviceCache = new RegisteredServiceCache(services, instanceProvider);
            _rootServiceProvider = new ServiceProvider(this, _serviceCache.ScopedCount, isRoot: true);
            _cachedInstances = new object[_serviceCache.SingletonCount + _serviceCache.ScopedCount];
        }



        public Abstractions.IServiceProvider CreateScope()
        {
            return new ServiceProvider(this, _serviceCache.ScopedCount, isRoot: false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetService<T>() where T : class
        {
            var registeredService = _serviceCache.GetRegisteredService<T>();
            var cachedKey = _serviceCache.ServiceKeyMapper[ServiceKey<T>.Id];

            switch (registeredService.Scope)
            {
                case Scope.Singleton:
                case Scope.Scoped:

                    if (_cachedInstances[cachedKey] is T singletonObj)
                    {
                        return singletonObj;
                    }

                    _cachedInstances[cachedKey] = registeredService.InstanceFactory(_rootServiceProvider);

                    return (T)_cachedInstances[cachedKey];

                case Scope.Transient:
                    return (T)registeredService.InstanceFactory(_rootServiceProvider);
            }

            throw new Exception($"Scope for type {typeof(T)} not found");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal T GetService<T>(ServiceProvider serviceProvider)
        {
            var registeredService = _serviceCache.GetRegisteredService<T>();
            var cachedKey = _serviceCache.ServiceKeyMapper[ServiceKey<T>.Id];

            switch (registeredService.Scope)
            {
                case Scope.Singleton:

                    if (_cachedInstances[cachedKey] is T singletonObj)
                    {
                        return singletonObj;
                    }

                    _cachedInstances[cachedKey] = registeredService.InstanceFactory(serviceProvider);

                    return (T)_cachedInstances[cachedKey];

                case Scope.Scoped:

                    if (serviceProvider.CachedInstances[cachedKey] is T scopedObj)
                    {
                        return scopedObj;
                    }

                    serviceProvider.CachedInstances[cachedKey] = registeredService.InstanceFactory(serviceProvider);

                    return (T)serviceProvider.CachedInstances[cachedKey]!;

                case Scope.Transient:
                    return (T)registeredService.InstanceFactory(serviceProvider);
            }

            throw new Exception($"Scope for type {typeof(T)} not found");
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