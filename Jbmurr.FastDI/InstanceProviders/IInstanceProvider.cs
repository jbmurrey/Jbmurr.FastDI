using Jbmurr.FastDI.ServicePlans;

namespace Jbmurr.FastDI.InstanceProviders
{
    internal interface IInstanceProvider
    {
        Func<ServiceProvider, object> Get(ServicePlan servicePlan);
    }
}