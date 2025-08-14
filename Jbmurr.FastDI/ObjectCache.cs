namespace Jbmurr.FastDI
{
    internal class ObjectCache
    {
        private readonly KeyStore _keyStore = new();
        private readonly object[] _cachedInstances;
        internal ObjectCache(Type[] types)
        {
            _cachedInstances = new object[types.Length];
            _keyStore.PopulateKeys(types);
        }

        internal T GetOrAdd<T>(Func<object> instanceProvider)
        {
            var slot = _keyStore.Slot<T>();

            if (_cachedInstances[slot] is T obj)
                return obj;

            _cachedInstances[slot] = (T)instanceProvider();

            return (T)_cachedInstances[slot];
        }

        internal void Clear()
        {
            Array.Clear(_cachedInstances, 0, _cachedInstances.Length);
        }
    }
}
