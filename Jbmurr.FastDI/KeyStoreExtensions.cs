using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jbmurr.FastDI
{
    internal static class KeyStoreExtensions
    {
        internal static KeyStoreBuilder PopulateKeys(this KeyStoreBuilder store, IEnumerable<Type> types)
        {
            int index = 0;
            foreach (var type in types)
            {
                store.Slot(type) = index;
                index++;
            }

            return store;   
        }
    }
}
