namespace SRF.Helpers
{
    using System;
    using System.Linq;
    using System.Reflection;    

    public class PropertyReference : IPropertyReference
    {
        private readonly PropertyInfo _property;
        private readonly object _target;

        public PropertyReference(object target, PropertyInfo property)
        {
            SRDebugUtil.AssertNotNull(target);

            _target = target;
            _property = property;
        }

        public string PropertyName
        {
            get { return _property.Name; }
        }

        public Type PropertyType
        {
            get { return _property.PropertyType; }
        }

        public bool CanRead
        {
            get
            {
#if NETFX_CORE
				return _property.GetMethod != null && _property.GetMethod.IsPublic;
#else
                return _property.GetGetMethod() != null;
#endif
            }
        }

        public bool CanWrite
        {
            get
            {
#if NETFX_CORE
				return _property.SetMethod != null && _property.SetMethod.IsPublic;
#else
                return _property.GetSetMethod() != null;
#endif
            }
        }

        public object GetValue()
        {
            if (_property.CanRead)
            {
                return SRReflection.GetPropertyValue(_target, _property);
            }

            return null;
        }

        public void SetValue(object value)
        {
            if (_property.CanWrite)
            {
                SRReflection.SetPropertyValue(_target, _property, value);
            }
            else
            {
                throw new InvalidOperationException("Can not write to property");
            }
        }

        public T GetAttribute<T>() where T : Attribute
        {
            var attributes = _property.GetCustomAttributes(typeof (T), true).FirstOrDefault();

            return attributes as T;
        }
    }

    public class FieldReference : IPropertyReference
    {
        private readonly FieldInfo _field;
        private readonly object _target;

        public FieldReference(object target, FieldInfo field)
        {
            SRDebugUtil.AssertNotNull(target);

            _target = target;
            _field = field;
        }

        public bool CanRead => true;
        public bool CanWrite => true;

        public string PropertyName => _field.Name;
        public Type PropertyType => _field.FieldType;

        public object GetValue()
        {
            return _field.GetValue(_target);     
        }

        public void SetValue(object value)
        {
            _field.SetValue(_target, value);
        }

        public T GetAttribute<T>() where T : Attribute
        {
            var attributes = _field.GetCustomAttributes(typeof(T), true).FirstOrDefault();

            return attributes as T;
        }
    }

    public class CustomReference : IPropertyReference
    {
        private object _target;
        private readonly string _name;
        private readonly Type _type;

        private readonly Func<object> _get;
        private readonly Action<object> _set;

        public CustomReference(Type type, string name, Func<object> get, Action<object> set)
        {
            SRDebugUtil.AssertNotNull(get);
            SRDebugUtil.AssertNotNull(set);

            _type = type;
            _name = name;
            _get = get;
            _set = set;
        }

        public bool CanRead => true;
        public bool CanWrite => true;

        public string PropertyName => _name;
        public Type PropertyType => _type;

        public object GetValue()
        {
            return _get();
        }

        public void SetValue(object value)
        {
            _set(value);
        }

        public T GetAttribute<T>() where T : Attribute
        {
            //var attributes = _target.GetCustomAttributes(typeof(T), true).FirstOrDefault();

            return null as T;
        }
    }
}
