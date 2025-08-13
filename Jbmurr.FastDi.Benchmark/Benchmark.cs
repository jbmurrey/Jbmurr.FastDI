using Autofac;
using Autofac.Util;
using BenchmarkDotNet.Attributes;
using DependencyExample;
using Jbmurr.FastDi.Benchmark;
using Jbmurr.FastDI.Abstractions;
using Jbmurr.FastDI.Tests;
using Microsoft.Extensions.DependencyInjection;
[MemoryDiagnoser]

public class Benchmark
{
    private Jbmurr.FastDI.Abstractions.IServiceProvider _myContainer;
    private Jbmurr.FastDI.Abstractions.IServiceProvider _myContainer2;
    private ServiceProvider _microsoftContainer;
    private IContainer _autofacContainer;


    [GlobalSetup]
    public void GlobalSetup()
    {


        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        services.AddTransient<DepA1a>();
        services.AddTransient<DepA2a>();
        services.AddTransient<DepB1a>();
        services.AddTransient<DepB2a>();
        services.AddTransient<DepC1a>();
        services.AddTransient<DepC2a>();
        services.AddTransient<DepD1a>();
        services.AddTransient<DepD2a>();
        services.AddTransient<DepA1>();
        services.AddTransient<DepA2>();
        services.AddTransient<DepB1>();
        services.AddTransient<DepB2>();
        services.AddTransient<DepC1>();
        services.AddTransient<DepC2>();
        services.AddTransient<DepD1>();
        services.AddTransient<DepD2>();
        services.AddTransient<DepA>();
        services.AddTransient<DepB>();
        services.AddTransient<DepC>();
        services.AddTransient<DepD>();
        services.AddTransient<MainClass>();


        // Build provider
        _microsoftContainer = services.BuildServiceProvider();

        var myServices = new Jbmurr.FastDI.Abstractions.ServiceCollection();


        myServices.AddTransient<DepA1a>();
        myServices.AddTransient<DepA2a>();
        myServices.AddTransient<DepB1a>();
        myServices.AddTransient<DepB2a>();
        myServices.AddTransient<DepC1a>();
        myServices.AddTransient<DepC2a>();
        myServices.AddTransient<DepD1a>();
        myServices.AddTransient<DepD2a>();
        myServices.AddTransient<DepA1>();
        myServices.AddTransient<DepA2>();
        myServices.AddTransient<DepB1>();
        myServices.AddTransient<DepB2>();
        myServices.AddTransient<DepC1>();
        myServices.AddTransient<DepC2>();
        myServices.AddTransient<DepD1>();
        myServices.AddTransient<DepD2>();
        myServices.AddTransient<DepA>();
        myServices.AddTransient<DepB>();
        myServices.AddTransient<DepC>();
        myServices.AddTransient<DepD>();
        myServices.AddTransient<MainClass>();

        // Build provider
        _myContainer = myServices.BuildServiceProvider();

        var builder = new ContainerBuilder();

        // Leaf level
        builder.RegisterType<DepA1a>().InstancePerDependency();
        builder.RegisterType<DepA2a>().InstancePerDependency();
        builder.RegisterType<DepB1a>().InstancePerDependency();
        builder.RegisterType<DepB2a>().InstancePerDependency();
        builder.RegisterType<DepC1a>().InstancePerDependency();
        builder.RegisterType<DepC2a>().InstancePerDependency();
        builder.RegisterType<DepD1a>().InstancePerDependency();
        builder.RegisterType<DepD2a>().InstancePerDependency();

        // Mid level
        builder.RegisterType<DepA1>().InstancePerDependency();
        builder.RegisterType<DepA2>().InstancePerDependency();
        builder.RegisterType<DepB1>().InstancePerDependency();
        builder.RegisterType<DepB2>().InstancePerDependency();
        builder.RegisterType<DepC1>().InstancePerDependency();
        builder.RegisterType<DepC2>().InstancePerDependency();
        builder.RegisterType<DepD1>().InstancePerDependency();
        builder.RegisterType<DepD2>().InstancePerDependency();

        // Top level
        builder.RegisterType<DepA>().InstancePerDependency();
        builder.RegisterType<DepB>().InstancePerDependency();
        builder.RegisterType<DepC>().InstancePerDependency();
        builder.RegisterType<DepD>().InstancePerDependency();

        // Root
        builder.RegisterType<MainClass>().SingleInstance();
        // Build the container
        _autofacContainer = builder.Build();


        var container = new Jbmurr.FastDI.Abstractions.ServiceCollection();
        container.AddSingleton<Jbmurr.FastDI.Tests.Disposable>();
        container.AddScoped<ImplClass2>();

        _myContainer2 = container.BuildServiceProvider();


    }

    [Benchmark]
    public void Microsoft()
    {
        using (var scope = _microsoftContainer.CreateScope())
        {
            var service = scope.ServiceProvider.GetService<MainClass>();
        }
    }

    //[Benchmark]
    //public void Mine()
    //{

    //    var x = new RecursiveCost();

    //    x.NTimes(1000);
    //}


    [Benchmark]
    public void Mine2()
    {

        using (var scope = _myContainer.CreateScope())
        {
            scope.GetService<MainClass>();
        }
    }
    //[Benchmark]
    //public void Autofac()
    //{
    //    //using (var scope = _autofacContainer.BeginLifetimeScope())
    //    //{
    //    //    scope.Resolve<MainClass>();
    //    //}
    //}

    //[Benchmark]
    //public void NoContainerSingletons()
    //{

    //}
}