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

            Assert.AreNotEqual(service1, service2);
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

            Assert.AreEqual(service1, service2);
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
            myServices.AddTransient<DepA1a>();
            myServices.AddTransient<DepA2a>();
            myServices.AddTransient<DepB1a>();
            myServices.AddTransient<DepB2a>();
            myServices.AddSingleton<DepC1a>();
            myServices.AddTransient<DepC2a>();
            myServices.AddTransient<DepD1a>();
            myServices.AddTransient<DepD2a>();

            // Mid level
            myServices.AddTransient<DepA1>();
            myServices.AddTransient<DepA2>();
            myServices.AddTransient<DepB1>();
            myServices.AddTransient<DepB2>();
            myServices.AddTransient<DepC1>();
            myServices.AddScoped<DepC2>();
            myServices.AddTransient<DepD1>();
            myServices.AddTransient<DepD2>();

            // Top level dependencies
            myServices.AddScoped<DepA>();
            myServices.AddTransient<DepB>();
            myServices.AddScoped<DepC>();
            myServices.AddTransient<DepD>();

            // Root
            myServices.AddTransient<MainClass>();

            // Build provider
            var sp = myServices.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
               var x = scope.GetService<MainClass>();
                scope.GetService<DepA1a>();
            }
        

        }
    }
}