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

            // Leaf level
            myServices.AddScoped<DepA1a>();
            myServices.AddScoped<DepA2a>();
            myServices.AddScoped<DepB1a>();
            myServices.AddScoped<DepB2a>();    
            myServices.AddScoped<DepC1a>();
            myServices.AddScoped<DepC2a>();
            myServices.AddScoped<DepD1a>();
            myServices.AddScoped<DepD2a>();

            // Mid level
            myServices.AddSingleton<DepA1>();
            myServices.AddSingleton<DepA2>();
            myServices.AddSingleton<DepB1>();
            myServices.AddSingleton<DepB2>();

            myServices.AddSingleton<DepC1>();
            myServices.AddSingleton<DepC2>();
            myServices.AddSingleton<DepD1>();
            myServices.AddSingleton<DepD2>();

            // Top level dependencies
            myServices.AddTransient<DepA>();
            myServices.AddTransient<DepB>();
            myServices.AddTransient<DepC>();
            myServices.AddTransient<DepD>();

            // Root
            myServices.AddTransient<MainClass>();

            // Build provider
            var sp = myServices.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var yy = scope.GetService<MainClass>();

  


        }


    }
}