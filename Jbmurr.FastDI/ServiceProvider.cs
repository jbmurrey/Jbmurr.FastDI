using System.Buffers;
using System.Runtime.CompilerServices;

namespace Jbmurr.FastDI
{
    public sealed class ServiceProvider(RootServiceProvider rootServiceProvider, int scopedCount, bool isRoot = false) : Abstractions.IServiceProvider
    {
        internal HashSet<IDisposable>? _disposableInstances;
        internal HashSet<IDisposable> DisposibleInstances => _disposableInstances ??= [];
        internal bool IsRoot { get; } = isRoot;
        internal object[]? _objectCache;
        internal object[] ObjectCache => _objectCache ??= new object[scopedCount];

        private readonly RootServiceProvider _rootServiceProvider = rootServiceProvider;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetService<T>() where T : class => _rootServiceProvider.GetService<T>(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Abstractions.IServiceProvider CreateScope() => new ServiceProvider(_rootServiceProvider, scopedCount);

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