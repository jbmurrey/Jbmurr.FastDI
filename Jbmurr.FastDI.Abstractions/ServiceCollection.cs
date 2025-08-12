using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Jbmurr.FastDI.Abstractions
{
    public class ServiceCollection : IEnumerable<Service>
    {
        internal readonly ConcurrentDictionary<Type, Service> _services = new();
      
        public IEnumerator<Service> GetEnumerator()
        {
            return _services.Select(x=>x.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
