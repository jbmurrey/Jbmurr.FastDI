using Jbmurr.FastDI.Abstractions;
using System.Reflection;

namespace Jbmurr.FastDI.ServicePlans
{
    internal class ConstructorPlan(Service service, ConstructorInfo constructorInfo, ServicePlan[] constructorParameters, int cacheKey) : ServicePlan(service, cacheKey)
    {
        internal ConstructorInfo ConstructorInfo { get; } = constructorInfo;
        internal ServicePlan[] ConstructorParameters { get; } = constructorParameters;
    }
}
