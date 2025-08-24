using Jbmurr.FastDI.Abstractions;
using Jbmurr.FastDI.ServicePlans;

namespace Jbmurr.FastDI
{
    internal static class ServiceProviderExtensions
    {
        internal static Dictionary<int, object>? FindServiceCache(this ServiceProvider serviceProvider, ServicePlan servicePlan)
        {
            if ((serviceProvider.IsRoot && servicePlan.Service.Scope == Scope.Scoped) || servicePlan.Service.Scope == Scope.Singleton)
            {
                return serviceProvider.RootServiceProvider.ObjectCache;
            }

            if (!serviceProvider.IsRoot && servicePlan.Service.Scope == Scope.Scoped)
            {
                return serviceProvider.ObjectCache;
            }

            return null;
        }
    }
}
