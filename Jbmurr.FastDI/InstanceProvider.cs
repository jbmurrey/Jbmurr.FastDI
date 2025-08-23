using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Jbmurr.FastDI
{
    internal class InstanceProvider : IInstanceProvider
    {
        public static ConcurrentDictionary<Type, ConstructorInfo> _cachedConstructorInfo = new();
        public static ConcurrentDictionary<ConstructorInfo, ParameterInfo[]> _cachedParameterInfo = new();

        public Func<ServiceProvider, object> Get(Type type)
        {
            var constructorInfo = _cachedConstructorInfo.GetOrAdd(type, type.GetConstructors()
                 .OrderByDescending(c => c.GetParameters().Length)
                 .FirstOrDefault()
                 ?? throw new InvalidOperationException($"No public constructors found for {type}."));

            return (serviceProvider) => Get(serviceProvider, constructorInfo);
        }

        private static object Get(ServiceProvider serviceProvider, ConstructorInfo constructorInfo)
        {
            var parameters = _cachedParameterInfo.GetOrAdd(constructorInfo, (cInfo) => cInfo.GetParameters());

            object[] arguments = new object[parameters.Length];

            for (int index = 0; index < parameters.Length; index++)
            {
                arguments[index] = serviceProvider.GetService(parameters[index].ParameterType);
            }

            return constructorInfo.Invoke(arguments);
        }
    }
}
