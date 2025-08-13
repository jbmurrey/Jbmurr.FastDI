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

        internal RootServiceProvider(ServiceCollection serviceCollection, IInstanceProvider instanceProvider)
        {
            _rootServiceProvider = new ServiceProvider(this);
            _serviceCache = new RegisteredServiceCache(serviceCollection, instanceProvider);
        }



        public Abstractions.IServiceProvider CreateScope()
        {
            return new ServiceProvider(this, isRoot: true);
        }

        public T GetService<T>() where T : class
        {
           //// var registeredService = _serviceCache.GetRegisteredService<T>();
           object? instance = null;

           // switch (registeredService.Scope)
           // {
           //     case Scope.Singleton:
           //     case Scope.Scoped:
           //         break;
           //     case Scope.Transient:
           //         instance = registeredService.InstanceFactory(_rootServiceProvider);
           //         break;

           // }

            return (T)instance!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal T GetService<T>(ServiceProvider serviceProvider)
        {
            var registeredService = _serviceCache.GetRegisteredService<T>();
            object? instance = null;

            switch (registeredService.Scope)
            {
                case Scope.Singleton:
                case Scope.Scoped:
                    break;
                case Scope.Transient:
                    instance = registeredService.InstanceFactory(_rootServiceProvider);
                    break;

            }

            return (T)instance!;
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