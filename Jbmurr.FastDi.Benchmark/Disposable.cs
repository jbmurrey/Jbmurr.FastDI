using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jbmurr.FastDI.Tests
{
    internal class Disposable : IDisposable
    {
        public bool IsDisposed { get; private set; }
        public int CalledCount { get; private set; }
        public void Dispose()
        {
            IsDisposed = true;
            CalledCount++;
        }
    }
}
