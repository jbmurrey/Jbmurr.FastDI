public static class TypeIndex
{
    private static int _next;
    public static int Count => _next;

    internal static int Allocate()
        => Interlocked.Increment(ref _next) - 1;
}
