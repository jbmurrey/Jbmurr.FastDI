using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Jbmurr.FastDI
{
    public class InstanceProvider : IInstanceProvider
    {
        private static readonly MethodInfo GetServiceGenericMethod =
            typeof(Abstractions.IServiceProvider)
                .GetMethods()
                .First(m => m.Name == nameof(Abstractions.IServiceProvider.GetService) && m.IsGenericMethodDefinition);

        // Change the IInstanceProvider interface to match:
        // Func<Abstractions.IServiceProvider, object> Get(Type type);
        public Func<Abstractions.IServiceProvider, object> Get(Type type)
        {
            var constructor = type.GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault()
                ?? throw new InvalidOperationException($"No public constructors found for {type}.");

            var spParam = Expression.Parameter(typeof(Abstractions.IServiceProvider), "sp");

            var args = constructor.GetParameters()
                .Select(p =>
                    Expression.Call(spParam, GetServiceGenericMethod.MakeGenericMethod(p.ParameterType)));

            var newExpr = Expression.New(constructor, args);
            var body = Expression.Convert(newExpr, typeof(object));

            var lambda = Expression.Lambda<Func<Abstractions.IServiceProvider, object>>(body, spParam);
            return lambda.Compile();
        }
    }
}
