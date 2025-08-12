using Jbmurr.FastDI.Abstractions;
using System.Collections.Concurrent;

namespace Jbmurr.FastDI
{
    internal class CategorizedServiceCollection
    {
        internal CategorizedServiceCollection(ServiceCollection services)
        {
            SingletonServices = new(services.ToConcurrentDictionary().Where(x => x.Value.Scope == Scope.Singleton));
            ScopedServices = new(services.ToConcurrentDictionary().Where(x => x.Value.Scope == Scope.Scoped));
            TransientServices = new(services.ToConcurrentDictionary().Where(x => x.Value.Scope == Scope.Transient));
            Combined = services.ToConcurrentDictionary();
        }

        internal ConcurrentDictionary<Type, Service> SingletonServices { get; }
        internal ConcurrentDictionary<Type, Service> ScopedServices { get; }
        internal ConcurrentDictionary<Type, Service> TransientServices { get; }
        internal ConcurrentDictionary<Type, Service> Combined { get; }
    }
}
