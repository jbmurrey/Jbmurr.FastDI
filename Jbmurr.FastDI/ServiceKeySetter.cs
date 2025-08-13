using System.Reflection;

namespace Jbmurr.FastDI
{
    internal static class ServiceKeySetter
    {
        internal static void Set(Type serviceType, int id)
        {
            var idPropertyInfo = typeof(ServiceKey<>)
                .MakeGenericType(serviceType)
                .GetProperty("Id", BindingFlags.NonPublic | BindingFlags.Static)!;

            idPropertyInfo.SetValue(null, id);
        }
    }
}