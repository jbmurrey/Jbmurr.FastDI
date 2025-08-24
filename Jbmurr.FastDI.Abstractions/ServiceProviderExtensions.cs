namespace Jbmurr.FastDI.Abstractions
{
    public static class ServiceProviderExtensions
    {
        public static object GetService<T>(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetService(typeof(T));
        }
    }
}
