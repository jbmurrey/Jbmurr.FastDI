using Jbmurr.FastDI.InstanceProviders;
using System.Runtime.CompilerServices;

namespace Jbmurr.FastDI.Abstractions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceProvider BuildServiceProvider(this ServiceCollection serviceCollection, bool useDynamicCodeGen = true)
        {
            if (RuntimeFeature.IsDynamicCodeSupported && useDynamicCodeGen)
            {
                return new RootServiceProvider(serviceCollection, new DynamicInstanceProvider());
            }

            return new RootServiceProvider(serviceCollection, new InstanceProvider());
        }
    }
}