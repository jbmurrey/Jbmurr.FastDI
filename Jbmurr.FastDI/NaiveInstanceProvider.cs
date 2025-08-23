//using Jbmurr.FastDI.Abstractions;

//namespace Jbmurr.FastDI
//{
//    internal class NaiveInstanceProvider : IInstanceProvider
//    {
//        public Func<ServiceProvider, object> Get(ServiceConstructionInfo serviceConstructionInfo)
//        {
//            return (serviceProvider) => Get(serviceProvider, serviceConstructionInfo);
//        }

//        private static object Get(ServiceProvider serviceProvider, ServiceConstructionInfo serviceConstructionInfo)
//        {
//            if (TryGetCachedObject(serviceProvider, serviceConstructionInfo.Service.Scope, serviceConstructionInfo.Key, out object? instance))
//            {
//                return instance!;
//            }

//            switch (serviceConstructionInfo)
//            {
//                case ConstructorConstructionInfo constructorConstructionInfo:
//                    object[] parameters = new object[constructorConstructionInfo.ConstructorParameters.Length];
                   
//                    for (int index = 0; index < parameters.Length; index++)
//                    {
//                        parameters[index] = Get(serviceProvider, constructorConstructionInfo.ConstructorParameters[index]);
//                    }

//                    return constructorConstructionInfo.Constructor.Invoke(parameters);

//                case FactoryConstructionInfo factoryConstructionInfo:
//                    return factoryConstructionInfo.InstanceFactory(serviceProvider);
//            }

//           throw new NotImplementedException();
//        }

//        private static bool TryGetCachedObject(ServiceProvider serviceProvider, Scope serviceScope, int key, out object? instance)
//        {
//            instance = null;

//            if ((serviceProvider.IsRoot && serviceScope == Scope.Scoped) || serviceScope == Scope.Singleton)
//            {
//                instance = serviceProvider.RootServiceProvider.ObjectCache[key];
//            }
//            else if (!serviceProvider.IsRoot && serviceScope == Scope.Scoped)
//            {
//                instance = serviceProvider.ObjectCache[key];
//            }

//            if (instance != null)
//            {
//                return true;
//            }

//            return false;
//        }
//    }
//}
