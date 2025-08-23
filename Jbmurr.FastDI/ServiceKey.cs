using System.Collections.Concurrent;
using System.Reflection;

internal static class ServiceKey<T>
{
    public static int Id = ServiceKeys.Allocate() - 1;

}

internal static class ServiceKeys
{
    private static int _count = 0;
    internal static int Count => _count;
    internal static int Allocate()
    {
        return Interlocked.Increment(ref _count);
    }
}

internal static class ServiceKey
{
    private static readonly ConcurrentDictionary<Type, Func<int>> _getters = new();

    public static int GetId(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var getter = _getters.GetOrAdd(type, static t =>
        {
            var open = typeof(ServiceKey).GetMethod(
                nameof(GetIdGeneric),
                BindingFlags.NonPublic | BindingFlags.Static)!;

            var closed = open.MakeGenericMethod(t);

            return (Func<int>)Delegate.CreateDelegate(typeof(Func<int>), closed);
        });

        return getter();
    }
    private static int GetIdGeneric<T>() => ServiceKey<T>.Id;
}
