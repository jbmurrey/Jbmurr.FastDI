using Jbmurr.FastDI.ServicePlans;

namespace Jbmurr.FastDI.InstanceProviders
{
    internal class NonRecursiveInstanceProvider : IInstanceProvider
    {
        public Func<ServiceProvider, object> Get(ServicePlan servicePlan)
        {
            throw new NotImplementedException();
        }
    }
}
