using System.Runtime.CompilerServices;

namespace Jbmurr.FastDI
{
    internal sealed class ServiceProvider : Abstractions.IServiceProvider
    {
        internal HashSet<IDisposable>? DisposibleInstances { get; set; }
        internal bool IsRoot {get;}
        private readonly RootServiceProvider _rootServiceProvider;
        

        internal ServiceProvider(RootServiceProvider rootServiceProvider,bool isRoot = false)
        {
            IsRoot = isRoot;
            _rootServiceProvider = rootServiceProvider;
        }

        public Abstractions.IServiceProvider CreateScope()
        {
            return new ServiceProvider(_rootServiceProvider);
        }
      
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetService<T>() where T : class
        {
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