using Autofac;
using Autofac.Util;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using DependencyExample;
using Jbmurr.FastDi.Benchmark;
using Jbmurr.FastDI;
using Jbmurr.FastDI.Abstractions;
using Jbmurr.FastDI.Tests;
using Microsoft.Extensions.DependencyInjection;
[MemoryDiagnoser]
[InliningDiagnoser(true,true)]
[DisassemblyDiagnoser(printSource: true, exportCombinedDisassemblyReport: true)]
public class Benchmark2
{
    private Jbmurr.FastDI.Abstractions.IServiceProvider _myContainer;



    [GlobalSetup]
    public void GlobalSetup()
    {


        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();



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
        myServices.AddTransient<MainClass>();
        // Build provider
        _myContainer = myServices.BuildServiceProvider();


    }

    [Benchmark]
    public void Mine()
    {
       
    }

}