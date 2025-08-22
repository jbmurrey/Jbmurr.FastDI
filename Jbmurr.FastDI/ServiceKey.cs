using System;
using System.Collections.Concurrent;
using System.Reflection;

public static class ServiceKey<T>
{
    public static int Id { get; internal set; }

    public static int Get() => Id;
}

public static class ServiceKey
{
    private static readonly ConcurrentDictionary<Type, FieldInfo> _cache = new();

    public static void Set(Type type, int index)
    {
        if (type is null) throw new ArgumentNullException(nameof(type));

        var field = _cache.GetOrAdd(type, t =>
        {
            var closed = typeof(ServiceKey<>).MakeGenericType(t);
            return closed.GetProperty(nameof(ServiceKey<int>.Id), BindingFlags.Public | BindingFlags.Static)!
                         .SetMethod
                         .DeclaringType!
                         .GetProperty(nameof(ServiceKey<int>.Id), BindingFlags.Public | BindingFlags.Static)!
                         .SetMethod
                         .DeclaringType!
                         .GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Static);
        });

        field!.SetValue(null, index);
    }
}
