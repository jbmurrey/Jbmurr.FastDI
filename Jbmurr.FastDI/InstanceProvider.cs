using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Jbmurr.FastDI
{
    internal class InstanceProvider : IInstanceProvider
    {
        private static readonly MethodInfo GetServiceGenericMethod =
            typeof(ServiceProvider)
                .GetMethods()
                .First(m => m.Name == nameof(ServiceProvider.GetService) && m.IsGenericMethodDefinition);
        public Func<ServiceProvider, object> Get(Type type)
        {
            var constructor = type.GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault()
                ?? throw new InvalidOperationException($"No public constructors found for {type}.");

            var spParam = Expression.Parameter(typeof(ServiceProvider), "sp");

            var args = constructor.GetParameters()
                .Select(p =>
                    Expression.Call(spParam, GetServiceGenericMethod.MakeGenericMethod(p.ParameterType)));

            var newExpr = Expression.New(constructor, args);
            var body = Expression.Convert(newExpr, typeof(object));

            var lambda = Expression.Lambda<Func<ServiceProvider, object>>(body, spParam);
            return lambda.Compile();
        }
    }
}
