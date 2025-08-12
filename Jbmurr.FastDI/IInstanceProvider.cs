namespace Jbmurr.FastDI
{
    public interface IInstanceProvider
    {
        Func<Abstractions.IServiceProvider,object> Get(Type type);
    }
}