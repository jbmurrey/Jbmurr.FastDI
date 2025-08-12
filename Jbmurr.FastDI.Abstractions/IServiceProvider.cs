namespace Jbmurr.FastDI.Abstractions
{
    public interface IServiceProvider : IDisposable
    {
        T GetService<T>() where T : class;
        IServiceProvider CreateScope();
    }
}
