using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jbmurr.FastDI.Tests.Models
{
    public class Circular(InnerCircle innerCircle)
    {

    }

    public class InnerCircle(Circular circular)
    {

    }
}
