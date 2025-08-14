namespace Jbmurr.FastDI
{
    public sealed class KeyStoreBuilder
    {
        private int[] _slots = [];

        public ref int Slot<T>()
        {
            int id = TypeId<T>.Id;
            if (id >= _slots.Length)
                Array.Resize(ref _slots, id + 1);
            return ref _slots[id];
        }

        // Non-generic access when you only have a Type at runtime
        public ref int Slot(Type t)
        {
            // Force TypeId<T> to initialize and fetch its Id
            var g = typeof(TypeId<>).MakeGenericType(t);
            int id = (int)g.GetField(nameof(TypeId<int>.Id))!.GetValue(null)!;

            if (id >= _slots.Length)
                Array.Resize(ref _slots, id + 1);
            return ref _slots[id];
        }

        public KeyStore Build()
        {
            return new KeyStore(_slots);
        }
    }

    public sealed class KeyStore(int[] slots)
    {
        private readonly int[] _slots = slots;
        public int Count => _slots.Length;

        public int Slot<T>()
        {
            return _slots[TypeId<T>.Id];
        }
        public ref int Slot(Type t)
        {
            var g = typeof(TypeId<>).MakeGenericType(t);
            int id = (int)g.GetField(nameof(TypeId<int>.Id))!.GetValue(null)!;      
            return ref _slots[id];
        }

    }
}