using System.Runtime.CompilerServices;

namespace Jbmurr.FastDI
{
    internal sealed class ServiceProvider : Abstractions.IServiceProvider
    {
        internal HashSet<IDisposable>? DisposibleInstances { get; set; }
        internal bool IsRoot { get; }
        internal object?[] CachedInstances { get; }
        private readonly RootServiceProvider _rootServiceProvider;


        internal ServiceProvider(RootServiceProvider rootServiceProvider, int scopedCount, bool isRoot = false)
        {
            IsRoot = isRoot;
            _rootServiceProvider = rootServiceProvider;
            CachedInstances = new object?[scopedCount];
        }

        public Abstractions.IServiceProvider CreateScope()
        {
            return new ServiceProvider(_rootServiceProvider, CachedInstances.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetService<T>() where T : class
        {
            if (IsRoot)
            {
                return _rootServiceProvider.GetService<T>();   
            }

            return _rootServiceProvider.GetService<T>(this);
        }

        public void Dispose()
        {
            if (DisposibleInstances is null)
            {
                return;
            }

            foreach (var disposibleInstance in DisposibleInstances)
            {
                disposibleInstance.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}