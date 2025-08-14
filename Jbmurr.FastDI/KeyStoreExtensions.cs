using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jbmurr.FastDI
{
    internal static class KeyStoreExtensions
    {
        internal static void PopulateKeys(this KeyStore store, IEnumerable<Type> types)
        {
            int index = 0;
            foreach (var type in types)
            {
                store.Slot(type) = index;
                index++;
            }
        }
    }
}
