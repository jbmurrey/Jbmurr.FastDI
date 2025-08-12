using System.Collections.Concurrent;

namespace Jbmurr.FastDI.Abstractions
{
    public static class ServiceExtensions
    {

        public static IServiceProvider BuildServiceProvider(this ServiceCollection serviceCollection, IInstanceProvider? provider = null)
        {
     
            return new RootServiceProvider(serviceCollection, provider ?? new InstanceProvider());
        }
    }
}