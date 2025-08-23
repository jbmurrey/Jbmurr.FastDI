namespace Jbmurr.FastDI.Abstractions
{
    public interface IServiceProvider : IDisposable
    {
        T GetService<T>() where T : class;
        object GetService(Type serviceType);
        IServiceProvider CreateScope();
    }
}
