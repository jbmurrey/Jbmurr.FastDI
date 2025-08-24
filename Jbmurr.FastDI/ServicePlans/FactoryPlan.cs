using Jbmurr.FastDI.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jbmurr.FastDI.ServicePlans
{
    internal class FactoryPlan(Service service, int cacheKey) : ServicePlan(service, cacheKey)
    {
        internal Func<ServiceProvider, object> InstanceFactory { get; } = service.InstanceFactory!;
    }
}
