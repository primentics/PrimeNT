using System;
using System.Reflection;

namespace AzyWorks.Configuration
{
    public class ConfigDescriptor
    {
        public Assembly Assembly { get; }

        public Type Type { get; }
        public Type ValueType { get; }

        public object Target { get; }
        public object Handle { get; }
        public object DefaultValue { get; }

        public string Name { get; }
        public string Description { get; }
        
        internal ConfigDescriptor(object target, object handle, Type owner, Type valueType, string name, string description)
        {
            if (target is null)
                throw new ArgumentNullException("target");

            Assembly = owner.Assembly;

            Type = owner;
            ValueType = valueType;

            Target = target;
            Handle = handle; 

            Name = name;
            Description = description;
            DefaultValue = GetValue();
        }

        public void SetValue(object value)
        {
            if (value != null)
            {
                var valueType = value.GetType();

                if (valueType != ValueType)
                {
                    Log.SendError("ConfigDescriptor", $"Value type mismatch in config: {Name} ({Type.FullName})");
                    return;
                }
            }

            if (Target is PropertyInfo property)
                property.SetValue(Handle, value);
            else if (Target is FieldInfo field)
                field.SetValue(Handle, value);
            else
                Log.SendError("ConfigDescriptor", $"Unknown target in config: {Name} ({Type.FullName})");
        }

        public object GetValue()
        {
            if (Target is PropertyInfo property)
                return property.GetValue(Handle);
            else if (Target is FieldInfo field)
                return field.GetValue(Handle);
            else
            {
                Log.SendError("ConfigDescriptor", $"Unknown target in config: {Name} ({Type.FullName})");
                return null;
            }
        }
    }
}
