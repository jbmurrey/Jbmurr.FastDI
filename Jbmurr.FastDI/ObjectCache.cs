namespace Jbmurr.FastDI
{
    public class ObjectCache(KeyStore keyStore)
    {
        private readonly KeyStore _keyStore = keyStore;
        private readonly object?[] _cachedInstances = new object?[keyStore!.Count];

        public T GetOrAdd<T>(Func<object> instanceProvider)
        {
            var slot = _keyStore.Slot<T>();

            object? cachedInstance = _cachedInstances[slot];

            if (cachedInstance != null)
            {
                return (T)cachedInstance;
            }

            var instance = (T)instanceProvider();
            _cachedInstances[slot] = instance;
            return instance;
        }

        public void Clear()
        {
            Array.Clear(_cachedInstances, 0, _cachedInstances.Length);
        }
    }
}
