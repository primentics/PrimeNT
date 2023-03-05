using AzyWorks.System;

using System;
using System.Collections.Generic;
using System.IO;

namespace AzyWorks.Networking
{
    public static class NetUtils
    {
        public static event Action<INetMessage> OnMessageCreated;
        public static event Action<NetPayload> OnPayloadRead;

        public static BinaryWriter Write(this BinaryWriter writer, Type type)
        {
            writer.Write(type.AssemblyQualifiedName);

            Log.SendDebug("NetUtils", $"Written type name: {type.AssemblyQualifiedName}");

            return writer;
        }

        public static Type ReadType(this BinaryReader reader)
        {
            var typeName = reader.ReadString();

            Log.SendDebug("NetUtils", $"Type Name: {typeName}");

            var type = Type.GetType(typeName);

            if (type is null)
                throw new TypeLoadException(typeName);

            return type;
        }

        public static bool TryReadPayload(ArraySegment<byte> data, out NetPayload payload)
        {
            var messages = new List<INetMessage>();

            using (var stream = new MemoryStream(data.Array))
            using (var reader = new BinaryReader(stream))
            {
                var size = reader.ReadInt32();
                for (int i = 0; i < size; i++)
                {
                    var type = reader.ReadType();
                    if (!TryGetMessage(type, out var msg))
                    {
                        Log.SendWarn("NetUtils", $"Failed to retrieve a message with type: {type.FullName}");
                        continue;
                    }

                    msg.Deserialize(reader);

                    messages.Add(msg);
                }
            }

            payload = new NetPayload(messages);

            OnPayloadRead?.Invoke(payload);

            return true;
        }

        public static bool TryGetMessage(Type type, out INetMessage message)
        {
            message = Reflection.Instantiate<INetMessage>(type);

            OnMessageCreated?.Invoke(message);

            return message != null;
        }
    }
}
