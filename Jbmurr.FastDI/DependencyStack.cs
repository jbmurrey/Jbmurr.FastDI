namespace Jbmurr.FastDI
{
    internal class DependencyStack 
    {
        internal Stack<Type> _stack = new();

        internal void Push(Type type)
        {
            if (_stack.Contains(type))
            {
                ThrowCircularDependency(type);
            }

            _stack.Push(type);
        }

        internal Type Pop()
        {
            return _stack.Pop();
        }

        internal static void ThrowCircularDependency(Type type)
        {
            throw new CircularDependencyException($"Circular Dependency Detected for type {type}");
        }
    }
}
