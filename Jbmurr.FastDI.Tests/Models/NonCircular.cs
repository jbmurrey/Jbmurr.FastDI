namespace Jbmurr.FastDI.Tests.Models
{
    public class NonCircular(Dependency dependency, Dependency dependency2)
    {
        private readonly Dependency dependency = dependency;
        private readonly Dependency dependency2 = dependency2;
    }
}
