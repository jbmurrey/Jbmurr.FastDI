namespace Jbmurr.FastDI
{
    public class CircularDependencyException : Exception
    {
        internal CircularDependencyException() { }
        internal CircularDependencyException(string message) : base(message) { }
    }
}
