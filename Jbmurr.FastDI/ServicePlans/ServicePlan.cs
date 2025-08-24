using Jbmurr.FastDI.Abstractions;

namespace Jbmurr.FastDI.ServicePlans
{
    internal abstract class ServicePlan(Service service, int cacheKey)
    {
        internal readonly Service Service = service;
        internal readonly int CacheKey = cacheKey;
    }
}

