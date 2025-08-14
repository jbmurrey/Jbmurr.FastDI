using System.Collections.Concurrent;

namespace Jbmurr.FastDI.Abstractions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceProvider BuildServiceProvider(this ServiceCollection serviceCollection)
        {

            return new RootServiceProvider(serviceCollection, new InstanceProvider());
        }

        public static Type[] GetScopedTypes(this ServiceCollection serviceCollection)
        {
            return serviceCollection
                .Where(x => x.Scope == Scope.Scoped)
                .Select(x => x.ServiceType)
                .ToArray();
        }
    }
}