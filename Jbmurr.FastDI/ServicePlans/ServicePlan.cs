using Jbmurr.FastDI.Abstractions;

namespace Jbmurr.FastDI.ServicePlans
{
    internal abstract class ServicePlan(Service service, int key)
    {
        internal readonly Service Service = service;
        internal readonly int Key = key;
    }
}

