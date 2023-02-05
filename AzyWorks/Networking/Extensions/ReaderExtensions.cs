using AzyWorks.Networking.Messages;
using AzyWorks.Utilities;

using LiteNetLib.Utils;

using System;

namespace AzyWorks.Networking.Extensions
{
    public static class ReaderExtensions
    {
        public static Type ReadType(this NetDataReader reader)
        {
            var typeName = reader.GetString();
            var type = Type.GetType(typeName);

            if (type is null)
                throw new TypeLoadException($"Failed to load type: {typeName}");

            return type;
        }

        public static INetworkMessage ReadMessage(this NetDataReader reader, Type messageType)
        {
            var message = ReflectUtils.Instantiate<INetworkMessage>(messageType);

            if (message is null)
                throw new Exception($"Failed create an instance of {messageType.FullName}");

            message.Deserialize(reader);

            return message;
        }

        public static INetworkMessage[] ReadMessages(this NetDataReader reader)
        {
            var messageCount = reader.GetInt();
            var messageArray = new INetworkMessage[messageCount];

            for (int i = 0; i < messageCount; i++)
            {
                var messageType = reader.ReadType();
                var message = reader.ReadMessage(messageType);
                messageArray[i] = message;
            }

            return messageArray;
        }
    }
}
