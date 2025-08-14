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
            myServices.AddScoped<MainClass>();

            myServices.AddSingleton<DepA>(sb =>
            {
                // === Level 3 instances ===
                var a1a = new DepA1a();
                var a2a = new DepA2a();
                var b1a = new DepB1a();
                var b2a = new DepB2a();
                var c1a = new DepC1a();
                var c2a = new DepC2a();
                var d1a = new DepD1a();
                var d2a = new DepD2a();

                // === Level 2 instances ===
                var a1 = new DepA1(a1a);
                var a2 = new DepA2(a2a);
                var b1 = new DepB1(b1a);
                var b2 = new DepB2(b2a);
                var c1 = new DepC1(c1a);
                var c2 = new DepC2(c2a);
                var d1 = new DepD1(d1a);
                var d2 = new DepD2(d2a);

                // === Level 1 instances ===
                return new DepA(a1, a2);
             
            });
            // Build provider
            var sp = myServices.BuildServiceProvider();
          

            sp.GetService<MainClass>();
            sp.GetService<MainClass> ();
        

        }

        [TestMethod]
        public void Key()
        {
            var key = new KeyStoreBuilder();
            key.Slot<int>() =5;
            key.Slot<string>() =5 ;
            var y = TypeId<string>.Id;
            var key2 = new KeyStoreBuilder();


        }
    }
}