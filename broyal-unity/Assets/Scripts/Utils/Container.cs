namespace Utils
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Collections.Generic;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class InjectAttribute : Attribute
    {
    }

    public interface IContainer
    {
        IContainer Parent { get; }

        void Register<T>(T t);
        void Register<T, T1>();
        void RegisterSingleton<T, T1>();

        T Resolve<T>();
        bool TryResolve(Type service, out object obj);
        object ResolveGeneric(Type genericType, Type arg);
        object Resolve(Type type);
        void Inject(object instance);
    }

    public class Container : IContainer
    {
        private readonly Dictionary<object, Type> generics;
        private readonly Dictionary<Type, Func<object>> services;
        private readonly List<IContainer> children;

        public IContainer Parent { get; private set; }

        public Container(IContainer parent = null)
        {
            this.generics = new Dictionary<object, Type>();
            this.services = new Dictionary<Type, Func<object>>();
            this.Parent = parent;
        }

        public void Register<T>(T instance)
        {
            Register(typeof (T), instance);
        }

        public void Register<T>( Func<T> instance )
        {
            Register(typeof(T), instance);
        }

        public void Register(Type serviceType, object instance)
        {
            services[serviceType] = () => instance;
        }

        public void Register<T>()
        {
            Register<T, T>();
        }

        public void Register<T, T1>()
        {
            Type serviceType = typeof (T);
            Type implimentationType = typeof (T1);

            if (serviceType.IsGenericTypeDefinition)
            {
                generics[serviceType] = implimentationType;
            }
            else
            {
                ConstructorInfo constructor = implimentationType.GetConstructors(BindingFlags.Instance | BindingFlags.Public).First();
                ParameterInfo[] parameterTypes = constructor.GetParameters();

                UnityEngine.Debug.Log(implimentationType + ".ctor -> " + parameterTypes.Aggregate("", (x, y) => x + y.ParameterType + ","));

                services[serviceType] = () =>
                {
                    var parameters = parameterTypes.Select(x => Resolve(x.ParameterType)).ToArray();
                    UnityEngine.Debug.Log(implimentationType + ".invoke -> " + parameters.Aggregate("", (x, y) => x + y.GetType() + ","));
                    return constructor.Invoke(parameters);
                };
            }
       
        }

        public void RegisterSingleton<T>()
        {
            RegisterSingleton<T, T>();
        }

        public void RegisterSingleton<T, T1>()
        {
            Type serviceType = typeof (T);
            Type implimentationType = typeof (T1);

            ConstructorInfo constructor = implimentationType.GetConstructors(BindingFlags.Instance | BindingFlags.Public).First();
            ParameterInfo[] parameterTypes = constructor.GetParameters();

            object instance = null;
            services[serviceType] = () =>
            {
                if (instance == null)
                {
                    var parameters = parameterTypes.Select(x => Resolve(x.ParameterType)).ToArray();
                    instance = constructor.Invoke(parameters);
                }
                return instance;
            };
        }

        public object ResolveGeneric(Type genericType, Type arg)
        {
            return Resolve(genericType.MakeGenericType(arg));
        }

        public T Resolve<T>()
        {
            return (T) Resolve(typeof (T));
        }

        public object Resolve(Type service)
        {
            if (services.ContainsKey(service))
            {
                return services[service]();
            }
            else if( service.IsGenericType && generics.ContainsKey(service.GetGenericTypeDefinition()))
            {
                var implementation = generics[service.GetGenericTypeDefinition()].MakeGenericType(service.GetGenericArguments());
                Register(service, implementation);
                return Resolve(service);
            }
            if (Parent != null)
            {
                return Parent.Resolve(service);
            }
            throw new Exception("Service not found: " + service);
        }
        
        public bool TryResolve(Type service, out object obj)
        {
            obj = null;
            
            if (services.ContainsKey(service))
            {
                obj = services[service]();
                return true;
            }

            if( service.IsGenericType && generics.ContainsKey(service.GetGenericTypeDefinition()))
            {
                var implementation = generics[service.GetGenericTypeDefinition()].MakeGenericType(service.GetGenericArguments());
                Register(service, implementation);
                obj = Resolve(service);
                return true;
            }
            
            if (Parent != null)
            {
                obj = Resolve(service);
                return true;
            }
            
            return false;
        }

        public void Inject(object instance)
        {
            var methods = instance.GetType()
                                  .GetMethods()
                                  .Where(x => x.GetCustomAttributes(typeof(InjectAttribute), false).Any());

            foreach (var method in methods)
            {
                var parameterTypes = method.GetParameters().Select(x => x.ParameterType);
                var parameters = parameterTypes.Select(x => Resolve(x)).ToArray();
                method.Invoke(instance, parameters);
            }
        }
    }
}