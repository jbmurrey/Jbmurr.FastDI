namespace Jbmurr.FastDI
{
    internal class DependencyStack 
    {
        internal Stack<int> _stack = new();

        internal void Push(int dependencyKey)
        {
            if (_stack.Contains(dependencyKey))
            {
                ThrowCircularDependency();
            }

            _stack.Push(dependencyKey);
        }

        internal int Pop()
        {
            return _stack.Pop();
        }

        internal static void ThrowCircularDependency()
        {
            throw new InvalidOperationException("Circular Dependency Detected");
        }
    }
}
