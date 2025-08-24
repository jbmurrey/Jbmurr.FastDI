using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Jbmurr.FastDI.Abstractions;
using Jbmurr.FastDI.ServicePlans;

namespace Jbmurr.FastDI.InstanceProviders
{
    internal sealed class DynamicInstanceProvider : IInstanceProvider
    {
        public Func<ServiceProvider, object> Get(ServicePlan servicePlan)
        {
            throw new NotImplementedException();
        }
    }
}
