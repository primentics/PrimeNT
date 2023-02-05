using System.Collections.Generic;

namespace AzyWorks.Configuration.Converters
{
    public class ConfigObject
    {
        private readonly Dictionary<string, object> _definedValues = new Dictionary<string, object>();

        public void AddValues(IEnumerable<ConfigDescriptor> descriptors)
        {
            foreach (var descriptor in descriptors)
            {
                _definedValues[descriptor.Name] = descriptor.GetValue();
            }
        }

        public void RemoveValues()
        {
            _definedValues.Clear();
        }

        public bool SetValues(IEnumerable<ConfigDescriptor> descriptors)
        {
            int faulted = 0;

            foreach (var descriptor in descriptors)
            {
                try
                {
                    if (_definedValues.TryGetValue(descriptor.Name, out var value))
                    {
                        descriptor.SetValue(value);
                    }
                }
                catch
                {
                    faulted++;
                }
            }

            if (faulted > 0)
                return false;

            return true;
        }
    }
}
