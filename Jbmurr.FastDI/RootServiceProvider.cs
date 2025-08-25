using Jbmurr.FastDI.Abstractions;
using Jbmurr.FastDI.InstanceProviders;
using Jbmurr.FastDI.ServicePlans;
using System.Collections.Concurrent;

namespace Jbmurr.FastDI
{
    public sealed class RootServiceProvider : Abstractions.IServiceProvider
    {
        private readonly HashSet<IDisposable> _disposibleInstances = [];
        private readonly ServiceProvider _rootServiceProvider;
        private readonly ServicePlanProvider _servicePlanProvider;
        private readonly IInstanceProvider _instanceProvider;

        internal RootServiceProvider(ServiceCollection serviceCollection, IInstanceProvider instanceProvider)
        {
            _rootServiceProvider = new ServiceProvider(this, true);
            _servicePlanProvider = new(serviceCollection);
            _instanceProvider = instanceProvider;
        }

        internal Dictionary<int, object> ObjectCache { get; } = [];

        public Abstractions.IServiceProvider CreateScope()
        {
            return new ServiceProvider(this);
        }

        public object GetService(Type serviceType) => GetService(_rootServiceProvider, serviceType);

        internal object GetService(ServiceProvider serviceProvider, Type serviceType)
        {
            var servicePlan = _servicePlanProvider.GetPlan(serviceType);

            return _instanceProvider.Get(servicePlan)(serviceProvider);
        }

        public void Dispose()
        {
            if (_disposibleInstances.Count == 0)
            {
                return;
            }

            foreach (var disposibleInstance in _disposibleInstances)
            {
                disposibleInstance.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}