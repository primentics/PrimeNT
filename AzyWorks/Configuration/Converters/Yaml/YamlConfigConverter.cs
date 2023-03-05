using System;

namespace AzyWorks.Configuration.Converters.Yaml
{
    public class YamlConfigConverter : IConfigConverter
    {
        public bool TryConvert(ConfigReader reader, Type resultType, out object result)
        {
            try
            {
                result = YamlParsers.Deserializer.Deserialize(reader.CurrentValue, resultType);
                return result != null;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        public bool TryConvert(ConfigWriter writer, string key, object value)
        {
            try
            {
                writer.SetOnKey(key);

                if (value is null)
                {
                    writer.Write("null\n");
                    return true;
                }

                var text = YamlParsers.Serializer.Serialize(value);

                writer.Write(text);
                writer.FinishKey();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
