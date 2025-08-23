using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Jbmurr.FastDI
{
    internal static class DependencyGuard<T>
    {
        [ThreadStatic]
        private static int _state;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Enter()
        {
            if (_state != 0)
                ThrowCircular();
            _state = 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Exit()
        {
            _state = 0;
        }

        [DoesNotReturn]
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowCircular() => throw new InvalidOperationException($"Circular dependency detected while resolving {typeof(T)}.");
    }
}
