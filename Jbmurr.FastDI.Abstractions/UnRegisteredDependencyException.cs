using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jbmurr.FastDI.Abstractions
{
    public class UnRegisteredDependencyException : Exception
    {
        public UnRegisteredDependencyException() { }
        public UnRegisteredDependencyException(string message) : base(message) { }
    }
}
