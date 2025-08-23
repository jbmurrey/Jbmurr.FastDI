using Jbmurr.FastDI.Abstractions;

namespace Jbmurr.FastDI
{
    internal class RegisteredService(Scope scope, Service service, Func<ServiceProvider, object> instanceProvider, int? cacheKey)
    {
        public Scope Scope { get; } = scope;
        public Service Service { get; } = service;
        public Func<ServiceProvider, object> InstanceFactory { get; } = instanceProvider;
        public int? CacheKey { get; } = cacheKey;
    }
}
