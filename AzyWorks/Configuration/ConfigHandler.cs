using AzyWorks.Configuration.Converters;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AzyWorks.Configuration
{
    public class ConfigHandler
    {
        private ConfigReader _reader;
        private ConfigWriter _writer;

        internal IConfigConverter _converter;
        internal HashSet<ConfigDescriptor> _configs = new HashSet<ConfigDescriptor>();

        public ConfigHandler(IConfigConverter configConverter)
        {
            _converter = configConverter;
        }

        public bool SaveToFile(string filePath)
        {
            if (_converter is null)
                return false;

            if (_configs.Count <= 0)
                return false;

            if (_writer is null)
                _writer = new ConfigWriter();

            _writer.ResetBuffer();

            foreach (var config in _configs)
            {
                if (!string.IsNullOrWhiteSpace(config.Description))
                    _writer.WriteDescription(config.Description);

                if (_converter.TryConvert(_writer, config.Name, config.GetValue()))
                    continue;
                else
                    Log.SendWarn("Config Handler", $"Failed to write key: {config.Name}");
            }

            var result = _writer.GetBuffer();

            File.WriteAllText(filePath, result);

            return true;
        }

        public bool LoadFromFile(string filePath)
        {
            if (_converter is null)
                return false;

            if (_configs.Count <= 0)
                return false;

            if (!File.Exists(filePath))
                return false;

            var lines = File.ReadAllLines(filePath);

            if (lines.Length > 0)
            {
                if (_reader is null)
                    _reader = new ConfigReader(this);

                _reader.SetBuffer(lines);

                return _reader.ReadConfig();
            }
            else
            {
                return false;
            }
        }

        public void RegisterConfigs(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
                RegisterConfigs(type);
        }

        public void RegisterConfigs(Type type, object handle = null)
        {
            RegisterFields(type, handle);
            RegisterProperties(type, handle);
        }

        public void RegisterConfigs(object obj)
            => RegisterConfigs(obj.GetType(), obj);

        private void RegisterFields(Type type, object handle = null)
        {
            foreach (var field in type.GetFields())
            {
                if (!TryGetAttribute(field, out var value))
                    continue;

                var descriptor = new ConfigDescriptor(
                    field,
                    handle,
                    type,

                    field.FieldType,

                    value.GetName(),
                    value.GetDescription());

                if (!_configs.Any(x => x == descriptor))
                    _configs.Add(descriptor);
            }
        }

        private void RegisterProperties(Type type, object handle = null)
        {
            foreach (var property in type.GetProperties())
            {
                if (!TryGetAttribute(property, out var value))
                    continue;

                var descriptor = new ConfigDescriptor(
                    property,
                    handle,
                    type,

                    property.PropertyType,

                    value.GetName(),
                    value.GetDescription());

                if (!_configs.Any(x => x == descriptor))
                    _configs.Add(descriptor);
            }
        }

        public void UnregisterConfigs()
        {
            _configs.Clear();
        }

        public bool UnregisterConfigs(Type type)
        {
            return _configs.RemoveWhere(x => x.Type == type) > 0;
        }

        public bool UnregisterConfigs(Assembly assembly)
        {
            return _configs.RemoveWhere(x => x.Assembly == assembly) > 0;
        }

        private bool TryGetAttribute(object target, out IConfigAttribute configAttribute)
        {
            if (target is PropertyInfo property)
            {
                var attribute = property.GetCustomAttributes()
                    .FirstOrDefault(x => x.GetType().GetInterfaces().Any(y => y == typeof(IConfigAttribute)));

                if (attribute is null)
                {
                    configAttribute = null;
                    return false;
                }
                else
                {
                    configAttribute = (IConfigAttribute)attribute;
                    return true;
                }
            }
            else if (target is FieldInfo field)
            {
                var attribute = field.GetCustomAttributes()
                    .FirstOrDefault(x => x.GetType().GetInterfaces().Any(y => y == typeof(IConfigAttribute)));

                if (attribute is null)
                {
                    configAttribute = null;
                    return false;
                }
                else
                {
                    configAttribute = (IConfigAttribute)attribute;
                    return true;
                }
            }

            configAttribute = null;
            return false;
        }
    }
}