namespace Jbmurr.FastDI.Abstractions
{
    public class Service
    {
        public Type ServiceType { get; }
        public Type ImplementationType { get; }
        public Scope Scope { get; }
        public Func<IServiceProvider,object>? InstanceFactory { get; }

        internal Service(Type serviceType, Type implementationType, Scope scope, Func<IServiceProvider,object>? instanceFactory)
        {
            ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
            ImplementationType = implementationType ?? throw new ArgumentNullException(nameof(implementationType));
            Scope = scope;
            InstanceFactory = instanceFactory;
        }
    }
}
