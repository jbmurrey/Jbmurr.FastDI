namespace Jbmurr.FastDI
{
    internal interface IInstanceProvider
    {
        Func<ServiceProvider,object> Get(Type type);
    }
}