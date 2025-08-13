using Jbmurr.FastDI.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Jbmurr.FastDI
{
    internal sealed class RootServiceProvider : Abstractions.IServiceProvider
    {
        private readonly ConcurrentDictionary<ServiceProvider, ConcurrentDictionary<Service, object>> _cachedScopedInstances = new();
        private readonly ConcurrentDictionary<Service, object> _cachedRootInstances = new();
        private readonly Func<ServiceProvider, object>[] _cachedInstanceProviders;
        private HashSet<IDisposable> _disposibleInstances = [];
        private readonly Service[] _serviceCollection;
        private readonly Service[] _keyedServiceCollection;
        private readonly IInstanceProvider _instanceProvider;
        private readonly ServiceProvider _rootServiceProvider;

        internal RootServiceProvider(ServiceCollection serviceCollection, IInstanceProvider instanceProvider)
        {
            _instanceProvider = instanceProvider;
            _rootServiceProvider = new ServiceProvider(this);   
            _serviceCollection = [.. serviceCollection];
            _keyedServiceCollection = new Service[_serviceCollection.Length];
            _cachedInstanceProviders =  new Func<ServiceProvider, object>[_serviceCollection.Length];
            AddInstanceProviders();
        }

        private void AddInstanceProviders()
        {
            int index = 0;  

            foreach (var service in _serviceCollection)
            {
                _cachedInstanceProviders[index] = _instanceProvider.Get(service.ImplementationType);
                _keyedServiceCollection[index] = service;
                ServiceKeySetter.Set(service.ServiceType, index);
                index++;
            }
        }

        public Abstractions.IServiceProvider CreateScope()
        {
            return new ServiceProvider(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetService<T>() where T : class
        {
            var service = _serviceCollection[ServiceKey<T>.Id];
            return (T)_cachedInstanceProviders[ServiceKey<T>.Id](_rootServiceProvider);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        internal T GetService<T>(ServiceProvider serviceProvider) where T : class
        {
            var service = _serviceCollection[ServiceKey<T>.Id];
            return (T)_cachedInstanceProviders[ServiceKey<T>.Id](_rootServiceProvider);
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