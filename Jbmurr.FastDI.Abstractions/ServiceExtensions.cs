using System.Collections.Concurrent;

namespace Jbmurr.FastDI.Abstractions
{
    public static class ServiceExtensions
    {
        public static void AddSingleton<T>(this ServiceCollection serviceCollection)

        {
            serviceCollection.AddSingleton<T, T>();
        }
        public static void AddSingleton<ServiceType, ImplementationType>(this ServiceCollection serviceCollection)
            where ImplementationType : ServiceType
        {
            serviceCollection._services[typeof(ServiceType)] = new Service(typeof(ServiceType), typeof(ImplementationType), Scope.Singleton, null);
        }

        public static void AddSingleton<ServiceType, ImplementationType>(this ServiceCollection serviceCollection, Func<object> instanceFactory)
            where ImplementationType : ServiceType
        {
            serviceCollection._services[typeof(ServiceType)] = new Service(typeof(ServiceType), typeof(ImplementationType), Scope.Singleton, instanceFactory);
        }
        public static void AddTransient<ServiceType>(this ServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ServiceType, ServiceType>();
        }

        public static void AddTransient<ServiceType, ImplementationType>(this ServiceCollection serviceCollection)
            where ImplementationType : ServiceType
        {
            serviceCollection._services[typeof(ServiceType)] = new Service(typeof(ServiceType), typeof(ImplementationType), Scope.Transient, null);
        }

        public static void AddTransient<ServiceType, ImplementationType>(this ServiceCollection serviceCollection, Func<object> instanceFactory)
            where ImplementationType : ServiceType
        {
            serviceCollection._services[typeof(ServiceType)] = new Service(typeof(ServiceType), typeof(ImplementationType), Scope.Transient, instanceFactory);
        }

        public static void AddScoped<T>(this ServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<T, T>();
        }

        public static void AddScoped<ServiceType, ImplementationType>(this ServiceCollection serviceCollection)
            where ImplementationType : ServiceType
        {
            serviceCollection._services[typeof(ServiceType)] = new Service(typeof(ServiceType), typeof(ImplementationType), Scope.Scoped, null);
        }

        public static void AddScoped<ServiceType, ImplementationType>(this ServiceCollection serviceCollection, Func<object> instanceFactory)
            where ImplementationType : ServiceType
        {
            serviceCollection._services[typeof(ServiceType)] = new Service(typeof(ServiceType), typeof(ImplementationType), Scope.Scoped, instanceFactory);
        }
    }
}