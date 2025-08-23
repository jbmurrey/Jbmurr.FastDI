using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Jbmurr.FastDI.Abstractions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceProvider BuildServiceProvider(this ServiceCollection serviceCollection)
        {
            if (RuntimeFeature.IsDynamicCodeSupported)
            {
                return new RootServiceProvider(serviceCollection, new DynamicInstanceProvider());
            }

            return new RootServiceProvider(serviceCollection,new InstanceProvider());   
        }
    }
}