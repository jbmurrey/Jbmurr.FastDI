using Jbmurr.FastDI.Abstractions;
using Jbmurr.FastDI.ServicePlans;

namespace Jbmurr.FastDI
{
    internal static class ServiceProviderExtensions
    {
        internal static object GetOrAddToCache(
            this ServiceProvider serviceProvider,
            ServicePlan servicePlan,
            Func<ServiceProvider, object> instanceFactory)
        {
            CachedInstances? cache = serviceProvider.FindServiceCache(servicePlan);

            if (cache == null)
            {
                return instanceFactory(serviceProvider);
            }

            return cache.CacheLocation switch
            {
                CacheLocation.Root => ThreadSafeGetOrAdd<RootServiceProvider>(cache.Values, servicePlan.Key, instanceFactory, serviceProvider),
                CacheLocation.Scope => ThreadSafeGetOrAdd<ServiceProvider>(cache.Values, servicePlan.Key, instanceFactory, serviceProvider),
                _ => throw new NotSupportedException($"CacheLocation of {cache.CacheLocation} is not supported."),
            };
        }

        private static object ThreadSafeGetOrAdd<T>(
            Dictionary<int, object> dictionary,
            int key,
            Func<ServiceProvider, object> factory,
            ServiceProvider sp)
        {
            lock (Lock<T>.Instance)
            {
                if (!dictionary.TryGetValue(key, out var value) || value == null)
                {
                    value = factory(sp);
                    dictionary[key] = value;
                }
                return value;
            }
        }


        private static CachedInstances? FindServiceCache(this ServiceProvider serviceProvider, ServicePlan servicePlan)
        {
            if ((serviceProvider.IsRoot && servicePlan.Service.Scope == Scope.Scoped) || servicePlan.Service.Scope == Scope.Singleton)
            {
                return new CachedInstances(serviceProvider.RootServiceProvider.ObjectCache, CacheLocation.Root);
            }

            if (!serviceProvider.IsRoot && servicePlan.Service.Scope == Scope.Scoped)
            {
                return new CachedInstances(serviceProvider.ObjectCache, CacheLocation.Scope);
            }

            return null;
        }

        private record CachedInstances(Dictionary<int, object> Values, CacheLocation CacheLocation) { }

        private enum CacheLocation
        {
            Scope,
            Root
        }
    }
}
