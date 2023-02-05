using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

namespace AzyWorks.IO.Binary
{
    public static class BinaryHelpers
    {
        public static void Write(this BinaryWriter writer, IBinaryObject binaryObject)
        {
            binaryObject.Serialize(writer);
        }

        public static T Read<T>(this BinaryReader reader) where T : IBinaryObject, new()
        {
            var obj = new T();

            obj.Deserialize(reader);

            return obj;
        }

        public static BinaryObject SerializeWithJson(object value)
        {
            var valueType = value.GetType();
            var binaryObject = new BinaryObject();

            binaryObject.QualifiedName = valueType.AssemblyQualifiedName;

            using (var outputStream = new MemoryStream())
            using (var writer = new BinaryWriter(outputStream))
            {
                writer.Write(JsonConvert.SerializeObject(value));

                binaryObject.Data = outputStream.ToArray();
            }

            return binaryObject;
        }

        public static object DeserializeWithJson(BinaryObject binaryObject)
        {
            var valueType = Type.GetType(binaryObject.QualifiedName);
            object result = null;

            using (var inputStream = new MemoryStream(binaryObject.Data))
            using (var reader = new BinaryReader(inputStream))
            {
                var json = reader.ReadString();

                result = JsonConvert.DeserializeObject(json, valueType);
            }

            return result;
        }

        public static BinaryObject SerializeObject(object value)
        {
            var valueType = value.GetType();
            var binaryObject = new BinaryObject();

            binaryObject.QualifiedName = valueType.AssemblyQualifiedName;

            using (var outputStream = new MemoryStream())
            using (var writer = new BinaryWriter(outputStream))
            {
                SerializeToWriter(value, valueType, writer);

                binaryObject.Data = outputStream.ToArray();
            }

            return binaryObject;
        }

        public static object DeserializeObject(BinaryObject obj)
        {
            var valueType = Type.GetType(obj.QualifiedName);
            object result = null;
            
            using (var inputStream = new MemoryStream(obj.Data))
            using (var reader = new BinaryReader(inputStream))
            {
                DeserializeWithReader(valueType, reader, out result);
            }

            return result;
        }

        private static void DeserializeWithReader(Type type, BinaryReader reader, out object result)
        {
            var flag = reader.ReadNextFlag();
            var interfaces = type.GetInterfaces();

            if (interfaces.Any(x => x == typeof(IBinaryObject)))
            {
                var binaryObject = (IBinaryObject)Activator.CreateInstance(type);

                binaryObject.Deserialize(reader);

                result = binaryObject;
            }
            else if (flag is BinaryNextFlag.NextNullValue || flag is BinaryNextFlag.NextUnknownValue || type == typeof(Nullable))
                result = null;
            else if (type == typeof(TimeSpan))
                result = TimeSpan.FromTicks(reader.ReadInt64());
            else if (type == typeof(DateTime))
                result = DateTime.FromBinary(reader.ReadInt64());
            else if (type.IsEnum || flag is BinaryNextFlag.NextEnumValue)
                result = Convert.ChangeType(reader.ReadInt32(), type);
            else if (interfaces.Any(x => x == typeof(IConvertible)))
                result = Convert.ChangeType(reader.ReadString(), type);
            else if (type == typeof(string))
                result = reader.ReadString();
            else
            {
                if (flag is BinaryNextFlag.NextJsonValue)
                {
                    type = Type.GetType(reader.ReadString());

                    var json = reader.ReadString();

                    result = JsonConvert.DeserializeObject(json, type);
                }
                else
                    result = null;
            }
        }

        private static BinaryNextFlag ReadNextFlag(this BinaryReader reader)
        {
            var flagByte = reader.ReadByte();
            var flag = (BinaryNextFlag) flagByte;

            return flag;
        }

        private static void WriteNextFlag(this BinaryWriter writer, BinaryNextFlag flag)
        {
            var flagByte = (byte)flag;

            writer.Write(flagByte);
        }

        private static void SerializeToWriter(object value, Type valueType, BinaryWriter writer)
        {
            if (value is IBinaryObject binaryObject)
            {
                writer.WriteNextFlag(BinaryNextFlag.NextBinaryObjectValue);
                binaryObject.Serialize(writer);
            }
            else if (value is null)
            {
                writer.WriteNextFlag(BinaryNextFlag.NextNullValue);
            }
            else if (value is TimeSpan timeSpan)
            {
                writer.WriteNextFlag(BinaryNextFlag.NextTimeSpanValue);
                writer.Write(timeSpan.Ticks);
            }
            else if (value is DateTime dateTime)
            {
                writer.WriteNextFlag(BinaryNextFlag.NextDateTimeValue);
                writer.Write(dateTime.ToBinary());
            }
            else if (valueType.IsEnum)
            {
                writer.WriteNextFlag(BinaryNextFlag.NextEnumValue);
                writer.Write(Convert.ToInt32(value));
            }
            else if (value is IConvertible convertible)
            {
                writer.WriteNextFlag(BinaryNextFlag.NextConvertibleValue);
                writer.Write(convertible.ToString());
            }
            else if (value is string str)
            {
                writer.WriteNextFlag(BinaryNextFlag.NextStringValue);
                writer.Write(str);
            }
            else
            {
                writer.WriteNextFlag(BinaryNextFlag.NextJsonValue);
                writer.Write(valueType.AssemblyQualifiedName);
                writer.Write(JsonConvert.SerializeObject(value));
            }
        }
    }
}