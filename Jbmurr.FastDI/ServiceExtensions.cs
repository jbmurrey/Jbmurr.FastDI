using System.Collections.Concurrent;

namespace Jbmurr.FastDI.Abstractions
{
    public static class ServiceExtensions
    {

        public static IServiceProvider BuildServiceProvider(this ServiceCollection serviceCollection, IInstanceProvider? provider = null)
        {
            var factories = new ConcurrentDictionary<Type, Func<Abstractions.IServiceProvider, object>>();

            // Give InstanceProvider a way to fetch already-built child factories
            var instanceProvider = new InstanceProvider(t =>
            {
                if (!factories.TryGetValue(t, out var f))
                    throw new InvalidOperationException($"Factory for {t} not built yet");
                return f;
            });

            return new RootServiceProvider(serviceCollection, instanceProvider);
        }
    }
}