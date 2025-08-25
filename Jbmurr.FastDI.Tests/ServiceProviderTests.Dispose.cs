using Jbmurr.FastDI.Abstractions;
using Jbmurr.FastDI.Tests.Models;

namespace Jbmurr.FastDI.Tests
{
    public partial class ServiceProviderTests
    {
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