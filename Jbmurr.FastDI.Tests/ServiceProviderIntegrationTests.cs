using DependencyExample;
using Jbmurr.FastDI.Abstractions;
using System.Diagnostics;

namespace Jbmurr.FastDI.Tests
{
    [TestClass]
    public class ServiceProviderIntegrationTests
    {
        [TestMethod]
        public void GivenTransientRegistrationsInstancesInSameScopeAreNotEqual()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient<IServiceClass, ImplClass>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();

            var service1 = scope.GetService<IServiceClass>();
            var service2 = scope.GetService<IServiceClass>();

            Assert.ReferenceEquals(service1, service2);
            Assert.IsInstanceOfType(service1, typeof(ImplClass));
            Assert.IsInstanceOfType(service2, typeof(ImplClass));
        }

        [TestMethod]
        public void GivenScopedRegistrationsInstancesInSameScopeAreEqual()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddScoped<IServiceClass, ImplClass>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();

            var service1 = scope.GetService<IServiceClass>();
            var service2 = scope.GetService<IServiceClass>();

            Assert.ReferenceEquals(service1, service2);
            Assert.IsInstanceOfType(service1, typeof(ImplClass));
            Assert.IsInstanceOfType(service2, typeof(ImplClass));
        }

        [TestMethod]
        public void GivenScopedRegistrationsInstancesInDifferentScopeAreNotEqual()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddScoped<IServiceClass, ImplClass>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            using var scope1 = serviceProvider.CreateScope();

            var service1 = scope1.GetService<IServiceClass>();

            using var scope2 = serviceProvider.CreateScope();
            var service2 = scope2.GetService<IServiceClass>();

            Assert.AreNotEqual(service1, service2);
            Assert.IsInstanceOfType(service1, typeof(ImplClass));
            Assert.IsInstanceOfType(service2, typeof(ImplClass));
        }

        [TestMethod]
        public void GivenSingletonRegistrationsInstancesInDifferentScopeAreEqual()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<IServiceClass, ImplClass>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            using var scope1 = serviceProvider.CreateScope();

            var service1 = scope1.GetService<IServiceClass>();

            using var scope2 = serviceProvider.CreateScope();
            var service2 = scope2.GetService<IServiceClass>();

            Assert.AreEqual(service1, service2);
            Assert.IsInstanceOfType(service1, typeof(ImplClass));
            Assert.IsInstanceOfType(service2, typeof(ImplClass));
        }

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
            myServices.AddTransient<DepD>();
            myServices.AddTransient<MainClass>();
            myServices.AddTransient<MainClass>();
            // Build provider
            myServices.BuildServiceProvider();
            var container = new Jbmurr.FastDI.Abstractions.ServiceCollection();
            container.AddSingleton<Jbmurr.FastDI.Tests.Disposable>();
            container.AddScoped<ImplClass2>();

            container.BuildServiceProvider();




        }


    }
}