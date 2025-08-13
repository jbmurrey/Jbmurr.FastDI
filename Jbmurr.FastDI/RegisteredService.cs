using Jbmurr.FastDI.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jbmurr.FastDI
{
    internal class RegisteredService
    {
        public RegisteredService(Scope scope, Service service, Func<ServiceProvider, object> instanceProvider)
        {
            Scope = scope;
            Service = service;
            InstanceFactory = instanceProvider;
        }

        public Scope Scope { get; }
        public Service Service { get; }
        public Func<ServiceProvider, object> InstanceFactory { get; }

    }
}
