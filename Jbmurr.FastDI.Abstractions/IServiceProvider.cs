namespace Jbmurr.FastDI.Abstractions
{
    public interface IServiceProvider : IDisposable
    {
        object GetService(Type serviceType);
        IServiceProvider CreateScope();
    }
}
