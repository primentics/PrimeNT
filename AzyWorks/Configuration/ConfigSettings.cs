using System.Collections.Generic;

namespace AzyWorks.Configuration
{
    public class ConfigSettings
    {
        private Dictionary<string, string> _registeredPaths = new Dictionary<string, string>();
        private Dictionary<string, string> _registeredPrefixes = new Dictionary<string, string>();

        public string DefaultPath { get; set; }
        public string DefaultFileName { get; set; }

        public void RegisterPath(string name, string path)
            => _registeredPaths[name] = path;

        public void RegisterPrefix(string name, string path)
            => _registeredPrefixes[name] = path;

        public bool TryGetPath(string name, out string path)
        {
            return _registeredPaths.TryGetValue(name, out path);
        }

        public bool TryGetPrefix(string name, out string prefix)
        {
            return _registeredPrefixes.TryGetValue(name, out prefix);
        }
    }
}
