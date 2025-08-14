namespace Jbmurr.FastDI
{
    public static class TypeId<T>
    {
        public static readonly int Id = TypeIndex.Allocate();
    }
}