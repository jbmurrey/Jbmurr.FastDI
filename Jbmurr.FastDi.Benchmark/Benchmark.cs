using Autofac;
using Autofac.Util;
using BenchmarkDotNet.Attributes;
using DependencyExample;
using Jbmurr.FastDi.Benchmark;
using Jbmurr.FastDI;
using Jbmurr.FastDI.Abstractions;
using Jbmurr.FastDI.Tests;
using Microsoft.Extensions.DependencyInjection;
[MemoryDiagnoser]

public class Benchmark
{
    private Jbmurr.FastDI.Abstractions.IServiceProvider _myContainer;
    private Jbmurr.FastDI.Abstractions.IServiceProvider _myContainer2;
    private Jbmurr.FastDI.Abstractions.ServiceCollection _services;
    private ServiceProvider _microsoftContainer;
    private IContainer _autofacContainer;
    private KeyStoreBuilder _keyStore;
    private KeyStore _readkeyStore;
    private ObjectCache _objectCache;
    private MainClass _mainClass;


    [GlobalSetup]
    public void GlobalSetup()
    {


        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        services.AddTransient<DepA1a>();
        services.AddTransient<DepA2a>();
        services.AddTransient<DepB1a>();
        services.AddTransient<DepB2a>();
        services.AddTransient<DepC1a>();
        services.AddScoped<DepC2a>();
        services.AddTransient<DepD1a>();
        services.AddTransient<DepD2a>();
        services.AddScoped<DepA1>();
        services.AddTransient<DepA2>();
        services.AddTransient<DepB1>();
        services.AddTransient<DepB2>();
        services.AddTransient<DepC1>();
        services.AddSingleton<DepC2>();
        services.AddTransient<DepD1>();
        services.AddTransient<DepD2>();
        services.AddTransient<DepA>();
        services.AddTransient<DepB>();
        services.AddTransient<DepC>();
        services.AddSingleton<DepD>();
        services.AddTransient<MainClass>();


        // Build provider
        _microsoftContainer = services.BuildServiceProvider();

        var myServices = new Jbmurr.FastDI.Abstractions.ServiceCollection();
        _services = myServices;

        myServices.AddTransient<DepA1a>();
        myServices.AddTransient<DepA2a>();
        myServices.AddTransient<DepB1a>();
        myServices.AddTransient<DepB2a>();
        myServices.AddTransient<DepC1a>();
        myServices.AddScoped<DepC2a>();
        myServices.AddTransient<DepD1a>();
        myServices.AddTransient<DepD2a>();
        myServices.AddScoped<DepA1>();
        myServices.AddTransient<DepA2>();
        myServices.AddTransient<DepB1>();
        myServices.AddTransient<DepB2>();
        myServices.AddTransient<DepC1>();
        myServices.AddSingleton<DepC2>();
        myServices.AddTransient<DepD1>();
        myServices.AddTransient<DepD2>();
        myServices.AddTransient<DepA>();
        myServices.AddTransient<DepB>();
        myServices.AddTransient<DepC>();
        myServices.AddSingleton<DepD>();
        myServices.AddTransient<MainClass>();
        myServices.AddTransient<MainClass>();
        // Build provider
        _myContainer = myServices.BuildServiceProvider();
        _mainClass = _myContainer.CreateScope().GetService<MainClass>();
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

        _keyStore = new KeyStoreBuilder();
        foreach (var service in myServices)
        {
            _keyStore.Slot(service.ServiceType);
        }

        _readkeyStore = _keyStore.Build();


       // _objectCache = new Jbmurr.FastDI.ObjectCache(myServices.Select(x => x.ServiceType).ToArray());


    }

    [Benchmark]
    public void Microsoft()
    {
        using (var container = _microsoftContainer.CreateScope())
        {
            container.ServiceProvider.GetService<MainClass>();
        }
    }

    [Benchmark]
    public void Mine()
    {
        using (var container = _myContainer.CreateScope())
        {
            container.GetService<MainClass>();
        }

    }


    //[Benchmark]
    //public void Mine2()
    //{

    //    _keyStore.Slot<DepA1a>();
    //    _keyStore.Slot<DepA2a>();
    //    _keyStore.Slot<DepB1a>();
    //    _keyStore.Slot<DepB2a>();
    //    _keyStore.Slot<DepC1a>();
    //    _keyStore.Slot<DepC2a>();
    //    _keyStore.Slot<DepD1a>();
    //    _keyStore.Slot<DepD2a>();
    //    _keyStore.Slot<DepA1>();
    //    _keyStore.Slot<DepA2>();
    //    _keyStore.Slot<DepB1>();
    //    _keyStore.Slot<DepB2>();
    //    _keyStore.Slot<DepC1>();
    //    _keyStore.Slot<DepC2>();
    //    _keyStore.Slot<DepD1>();
    //    _keyStore.Slot<DepD2>();
    //    _keyStore.Slot<DepA>();
    //    _keyStore.Slot<DepB>();
    //    _keyStore.Slot<DepC>();
    //    _keyStore.Slot<DepD>();
    //    _keyStore.Slot<MainClass>();
    //    _keyStore.Slot<MainClass>();
    //}

    //[Benchmark]
    //public void Readonly()
    //{
    //    int _next = 0;
    //    _next =Interlocked.Increment(ref _next) - 1;
    //}



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
    //    HashSet<IDisposable> hash = [];
    //}
}