using Jbmurr.FastDI.Abstractions;
using System;
using System.Collections.Concurrent;

namespace Jbmurr.FastDI
{
    internal class RootServiceProvider : Abstractions.IServiceProvider
    {
        private readonly ConcurrentDictionary<ServiceProvider, ConcurrentDictionary<Service, object>> _cachedScopedInstances = new();
        private readonly ConcurrentDictionary<Service, object> _cachedRootInstances = new();
        private readonly ConcurrentDictionary<Service, Func<ServiceProvider, object>> _cachedInstanceProviders = new();
        private HashSet<IDisposable> _disposibleInstances = [];
        private readonly CategorizedServiceCollection _categorizedServiceCollection;
        private readonly IInstanceProvider _instanceProvider;

        internal RootServiceProvider(ServiceCollection serviceCollection, IInstanceProvider instanceProvider)
        {
            _instanceProvider = instanceProvider;
            _categorizedServiceCollection = new CategorizedServiceCollection(serviceCollection);
            AddInstanceProviders();
        }

        private void AddInstanceProviders()
        {
            foreach (var service in _categorizedServiceCollection.Combined)
            {
                bool canAddInstanceProvider = _cachedInstanceProviders.TryAdd(service.Value, _instanceProvider.Get(service.Value.ImplementationType));

                if (!canAddInstanceProvider)
                {
                    throw new InvalidOperationException($"Failed to add instance provider for service {service.Value.ServiceType}.");
                }
            }
        }

        public Abstractions.IServiceProvider CreateScope()
        {
            return new ServiceProvider(this);
        }

        public T GetService<T>() where T : class
        {
            var service = _categorizedServiceCollection.Combined[typeof(T)];
            return null;

        }

        internal T GetService<T>(ServiceProvider serviceProvider) where T : class
        {
            var service = _categorizedServiceCollection.Combined[typeof(T)];
            return (T)_cachedInstanceProviders[service](serviceProvider);
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