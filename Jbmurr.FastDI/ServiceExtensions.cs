using System.Collections.Concurrent;

namespace Jbmurr.FastDI.Abstractions
{
    public static class ServiceExtensions
    {

        public static IServiceProvider BuildServiceProvider(this ServiceCollection serviceCollection)
        {

            return new RootServiceProvider(serviceCollection, new InstanceProvider());
        }
    }
}