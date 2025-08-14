using System.Buffers;
using System.Runtime.CompilerServices;

namespace Jbmurr.FastDI
{
    internal sealed class ServiceProvider : Abstractions.IServiceProvider
    {
        internal HashSet<IDisposable> DisposibleInstances { get; } = [];
        internal bool IsRoot { get; }
        internal  ObjectCache ObjectCache { get; }
        private readonly RootServiceProvider _rootServiceProvider;
       
        internal ServiceProvider(RootServiceProvider rootServiceProvider, ObjectCache scopedObjectCache, bool isRoot = false)
        {
            IsRoot = isRoot;
            _rootServiceProvider = rootServiceProvider;
            ObjectCache = scopedObjectCache;
        }

        public Abstractions.IServiceProvider CreateScope()
        {
            ObjectCache.Clear();
            return new ServiceProvider(_rootServiceProvider, ObjectCache);
        }

        public T GetService<T>() where T : class
        {
            if (IsRoot)
            {
                return _rootServiceProvider.GetService<T>();
            }

            return  _rootServiceProvider.GetService<T>(this);
        }

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