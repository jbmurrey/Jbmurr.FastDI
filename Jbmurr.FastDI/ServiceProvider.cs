using System.Buffers;
using System.Runtime.CompilerServices;

namespace Jbmurr.FastDI
{
    public sealed class ServiceProvider(RootServiceProvider rootServiceProvider, int scopedCount, bool isRoot = false) : Abstractions.IServiceProvider
    {
        internal HashSet<IDisposable> DisposibleInstances { get; } = [];
        internal object[] ObjectCache { get; } = new object[scopedCount];
        internal RootServiceProvider RootServiceProvider { get; } = rootServiceProvider;
        internal bool IsRoot { get; } = isRoot;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetService<T>() where T : class => RootServiceProvider.GetService<T>(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]  
        public Abstractions.IServiceProvider CreateScope() => new ServiceProvider(RootServiceProvider, scopedCount);
       
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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