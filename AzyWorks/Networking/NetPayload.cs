using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AzyWorks.Networking
{
    public class NetPayload
    {
        internal List<INetMessage> _messages;
        internal int pos;

        public IReadOnlyList<INetMessage> Messages { get => _messages; }

        public bool IsWriteable { get; }

        public NetPayload(IEnumerable<INetMessage> messages, bool isWriteable = true)
        {
            if (messages is List<INetMessage> list)
            {
                _messages = list;
                IsWriteable = isWriteable;
                return;
            }

            _messages = messages.ToList();
            IsWriteable = isWriteable;
        }

        public NetPayload(bool isWriteable = true) : this(new List<INetMessage>(), isWriteable) { }

        public NetPayload WithMessage(INetMessage message)
        {
            if (!IsWriteable)
            {
                Log.SendWarn("NetPayload", $"Attempted writing to a read-only payload.");
                return this;
            }

            _messages.Add(message);
            return this;
        }

        public NetPayload WithMessages(params INetMessage[] messages)
        {
            if (messages is null || messages.Length < 1)
                return this;

            foreach (var msg in messages)
                WithMessage(msg);

            return this;
        }

        public bool TryReadMessage<T>(out T message) where T : INetMessage
        {
            var res = _messages.FirstOrDefault(x => x is T);

            if (res is null)
            {
                message = default;
                return false;
            }

            message = (T)res;
            return true;
        }

        public bool TryReadMessages<T>(out IEnumerable<T> messages) where T : INetMessage
        {
            messages = _messages.Where(x => x is T t && t != null).Select(x => (T)x);
            return messages.Any();
        }

        public bool TryGetMessage(out INetMessage message)
        {
            if (pos >= _messages.Count)
            {
                message = null;
                return false;
            }

            message = _messages[pos];
            pos++;
            return message != null;
        }

        public void ToBeginning()
            => pos = 0;

        public void ToEnd()
            => pos = _messages.Count - 1;

        public ArraySegment<byte> ToSegment()
        {
            byte[] buffer = null;

            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(_messages.Count);
                for (int i = 0; i < _messages.Count; i++)
                {
                    writer.Write(_messages[i].GetType());
                    _messages[i].Serialize(writer);
                }

                buffer = stream.ToArray();
            }

            return new ArraySegment<byte>(buffer);
        }
    }
}