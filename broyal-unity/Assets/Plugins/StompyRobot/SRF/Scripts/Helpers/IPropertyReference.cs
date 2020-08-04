using System;

namespace SRF.Helpers
{
    public interface IPropertyReference
    {
        bool CanRead { get; }
        bool CanWrite { get; }
        string PropertyName { get; }
        Type PropertyType { get; }

        T GetAttribute<T>() where T : Attribute;
        object GetValue();
        void SetValue(object value);
    }
}