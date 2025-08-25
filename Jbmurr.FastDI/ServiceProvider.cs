using System.Collections.Concurrent;

namespace Jbmurr.FastDI
{
    public sealed class ServiceProvider(RootServiceProvider rootServiceProvider, bool isRoot = false) : Abstractions.IServiceProvider
    {
        internal HashSet<IDisposable> DisposibleInstances { get; } = [];
        internal Dictionary<int, object> ObjectCache { get; } = [];
        internal RootServiceProvider RootServiceProvider { get; } = rootServiceProvider;
        internal bool IsRoot { get; } = isRoot;

        public Abstractions.IServiceProvider CreateScope() => new ServiceProvider(RootServiceProvider);
        public object GetService(Type serviceType) => RootServiceProvider.GetService(this, serviceType);

        public void Dispose()
        {
            foreach (var disposibleInstance in DisposibleInstances)
            {
                disposibleInstance.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}