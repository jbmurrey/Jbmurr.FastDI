using DependencyExample;
using Jbmurr.FastDI.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Jbmurr.FastDI.Tests
{
    [TestClass,TestCategory("Integration")]
    public class ServiceProviderIntegrationTests
    {
        
        [TestMethod]
        public void Test()
        {
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
            myServices.AddSingleton<DepD>();
            myServices.AddSingleton<MainClass>();

            // Build provider


            var sp = myServices.BuildServiceProvider();

            using var s = sp.CreateScope();
            var instance = s.GetService<MainClass>();
            var instance2 = s.GetService<MainClass>();
            var t = s.GetService<DepC1>();






        }
    }
}