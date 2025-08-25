using Jbmurr.FastDI.Abstractions;
using Jbmurr.FastDI.Tests.Models;

namespace Jbmurr.FastDI.Tests
{
    public partial class ServiceProviderTests
    {
        [TestMethod]
        public void GivenCircularDependencyGetServiceThrows()
        {

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient<InnerCircle>();
            serviceCollection.AddTransient<Circular>();


            using var rootScope = serviceCollection.BuildServiceProvider(false);
            Assert.ThrowsException<CircularDependencyException>(rootScope.GetService<InnerCircle>);
        }

        [TestMethod]
        public void GivenNonCircularDependencyGetServiceDoesntThrows()
        {

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient<NonCircular>();
            serviceCollection.AddTransient<Dependency>();


            using var rootScope = serviceCollection.BuildServiceProvider(false);
            rootScope.GetService<NonCircular>();
        }
    }
}