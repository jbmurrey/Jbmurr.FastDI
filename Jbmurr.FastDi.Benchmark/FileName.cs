using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

public static class NoDynamicDi
{
    public static IServiceProvider BuildRuntimeEngineProvider(this IServiceCollection services, ServiceProviderOptions? options = null)
    {
        var spType = typeof(ServiceProvider);
        var asm = spType.Assembly;

        // internal interface/type names live in Microsoft.Extensions.DependencyInjection.ServiceLookup
        var iEngine = asm.GetType("Microsoft.Extensions.DependencyInjection.ServiceLookup.IServiceProviderEngine", throwOnError: true)!;
        var runtimeEngineType = asm.GetType("Microsoft.Extensions.DependencyInjection.ServiceLookup.RuntimeServiceProviderEngine", throwOnError: true)!;

        // ctor: RuntimeServiceProviderEngine(IServiceCollection services)
        var runtimeCtor = runtimeEngineType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                                           .First(ctor => {
                                               var ps = ctor.GetParameters();
                                               return ps.Length == 1 && ps[0].ParameterType == typeof(IServiceCollection);
                                           });

        var engine = runtimeCtor.Invoke(new object[] { services });

        // find non-public ServiceProvider ctor that takes (IServiceCollection, IServiceProviderEngine, ServiceProviderOptions)
        var spCtor = spType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
                           .First(ctor => {
                               var ps = ctor.GetParameters();
                               return ps.Length == 3
                                   && ps[0].ParameterType == typeof(IServiceCollection)
                                   && ps[1].ParameterType == iEngine
                                   && ps[2].ParameterType == typeof(ServiceProviderOptions);
                           });

        return (IServiceProvider)spCtor.Invoke(new object[] { services, engine, options ?? new ServiceProviderOptions() });
    }
}
