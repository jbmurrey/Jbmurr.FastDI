using Jbmurr.FastDI.Abstractions;
using Jbmurr.FastDI.Tests.Models;

namespace Jbmurr.FastDI.Tests
{

    public partial class ServiceProviderTests
    {
        [TestMethod]
        public void Given()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient<ClassWithDependency>();
            serviceCollection.AddSingleton<Dependency>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var service1 = serviceProvider.GetService<ClassWithDependency>();
            var service2 = serviceProvider.GetService<ClassWithDependency>();

            Assert.AreEqual(service1.Dependency, service2.Dependency);
        }
    }
}