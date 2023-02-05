using System;

namespace AzyWorks.Configuration.Converters
{
    public interface IConfigConverter
    {
        bool TryConvert(ConfigReader reader, Type resultType, out object result);
        bool TryConvert(ConfigWriter writer, string key, object value);
    }
}