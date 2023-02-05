using AzyWorks.Networking.Messages;

using LiteNetLib.Utils;

using System;
using System.Collections.Generic;
using System.Linq;

namespace AzyWorks.Networking.Extensions
{
    public static class WriterExtensions
    {
        public static void WriteType(this NetDataWriter writer, Type type)
        {
            writer.Put(type.AssemblyQualifiedName);
        }

        public static void WriteMessage(this NetDataWriter writer, INetworkMessage message)
        {
            writer.WriteType(message.GetType());
            message.Serialize(writer);
        }

        public static void WriteMessages(this NetDataWriter writer, IEnumerable<INetworkMessage> messages)
        {
            writer.Put(messages.Count());

            foreach (var message in messages)
            {
                writer.WriteMessage(message);
            }
        }
    }
}