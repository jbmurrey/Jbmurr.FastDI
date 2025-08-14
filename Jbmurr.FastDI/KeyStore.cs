namespace Jbmurr.FastDI
{
    public sealed class KeyStore
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
    }
}