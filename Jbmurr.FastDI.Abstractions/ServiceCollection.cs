using System.Collections;
using System.Collections.Concurrent;

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
