using Jbmurr.FastDI.Abstractions;

namespace Jbmurr.FastDI.Tests
{
    [TestClass]
    public class ServiceProviderTests
    {
        [TestMethod]
        public void GivenTransientRegistrationsInstancesInSameScopeAreNotEqual()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient<IServiceClass, ImplClass>();
            serviceCollection.AddScoped<IServiceClass2, ImplClass2>();
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
        public void GivenSingletonOnlyRootDisposes()
        {

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<Disposable>();
            Disposable root;

            using (var rootScope = serviceCollection.BuildServiceProvider())
            {
                root = rootScope.GetService<Disposable>();

                using (var scopedScope = rootScope.CreateScope())
                {
                    var scope = scopedScope.GetService<Disposable>();
                }

                Assert.IsFalse(root.IsDisposed);
            }

            Assert.IsTrue(root.IsDisposed);
        }

        [TestMethod]
        public void GivenScopedOnlyScopedProviderDisposes()
        {

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient<Disposable>();
 
            Disposable scope;
            using (var rootScope = serviceCollection.BuildServiceProvider())
            {                          
                using (var scopedScope = rootScope.CreateScope())
                {
                     scope = scopedScope.GetService<Disposable>();
                }

                Assert.IsTrue(scope.IsDisposed);
            }

            Assert.IsTrue(scope.CalledCount == 1);
        }
    }
}