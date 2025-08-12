using System;

namespace DependencyExample
{
    // === Level 3 ===
    public class DepA1a { }
    public class DepA2a { }
    public class DepB1a { }
    public class DepB2a { }
    public class DepC1a { }
    public class DepC2a { }
    public class DepD1a { }
    public class DepD2a { }

    // === Level 2 ===
    public class DepA1
    {
        private readonly DepA1a _a1a;
        public DepA1(DepA1a a1a) => _a1a = a1a;
    }

    public class DepA2
    {
        private readonly DepA2a _a2a;
        public DepA2(DepA2a a2a) => _a2a = a2a;
    }

    public class DepB1
    {
        private readonly DepB1a _b1a;
        public DepB1(DepB1a b1a) => _b1a = b1a;
    }

    public class DepB2
    {
        private readonly DepB2a _b2a;
        public DepB2(DepB2a b2a) => _b2a = b2a;
    }

    public class DepC1
    {
        private readonly DepC1a _c1a;
        public DepC1(DepC1a c1a) => _c1a = c1a;
    }

    public class DepC2
    {
        private readonly DepC2a _c2a;
        public DepC2(DepC2a c2a) => _c2a = c2a;
    }

    public class DepD1
    {
        private readonly DepD1a _d1a;
        public DepD1(DepD1a d1a) => _d1a = d1a;
    }

    public class DepD2
    {
        private readonly DepD2a _d2a;
        public DepD2(DepD2a d2a) => _d2a = d2a;
    }

    // === Level 1 ===
    public class DepA
    {
        private readonly DepA1 _a1;
        private readonly DepA2 _a2;
        public DepA(DepA1 a1, DepA2 a2)
        {
            _a1 = a1;
            _a2 = a2;
        }
    }

    public class DepB
    {
        private readonly DepB1 _b1;
        private readonly DepB2 _b2;
        public DepB(DepB1 b1, DepB2 b2)
        {
            _b1 = b1;
            _b2 = b2;
        }
    }

    public class DepC
    {
        private readonly DepC1 _c1;
        private readonly DepC2 _c2;
        public DepC(DepC1 c1, DepC2 c2)
        {
            _c1 = c1;
            _c2 = c2;
        }
    }

    public class DepD
    {
        private readonly DepD1 _d1;
        private readonly DepD2 _d2;
        public DepD(DepD1 d1, DepD2 d2)
        {
            _d1 = d1;
            _d2 = d2;
        }
    }

    // === Root ===
    public class MainClass
    {
        private readonly DepA _a;
        private readonly DepB _b;
        private readonly DepC _c;
        private readonly DepD _d;

        public MainClass(DepA a, DepB b, DepC c, DepD d)
        {
            _a = a;
            _b = b;
            _c = c;
            _d = d;
        }
    }
}
