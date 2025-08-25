using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jbmurr.FastDI.Tests.Models
{
    public class ClassWithDependency(Dependency dependency)
    {
        public Dependency Dependency { get; } = dependency;
    }

    public class Dependency()
    {

    }
}
