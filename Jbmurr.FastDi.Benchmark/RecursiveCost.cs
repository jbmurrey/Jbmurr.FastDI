using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jbmurr.FastDi.Benchmark
{
    internal class RecursiveCost
    {
        public void NTimes(int n)
        {
            if (n == 0)
                return;
            NTimes(n - 1);
        }
    }
}
