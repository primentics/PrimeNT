using AzyWorks.Logging;

using Ruffles.Connections;

using System;
using System.Collections.Generic;
using System.Net;

namespace AzyWorks.Networking.Server
{
    public class NetConnection
    {
        private HashSet<NetCallbackBase> _callbacks = new HashSet<NetCallbackBase>();

        private Connection _connection;
        private PersonalLog Log;

        public IPEndPoint EndPoint { get; }

        public ulong Id { get; }

        public int Ping { get => _connection?.Roundtrip ?? -1; }

        public bool Connected { get => _connection?.State is ConnectionState.Connected; }

        public event Action<NetPayload> OnPayloadProcessed;
        public event Action<NetPayload> OnPayloadSending;

        public event Action<ArraySegment<byte>> OnDataSending;

        public NetConnection(Connection connection)
        {
            _connection = connection;

            Id = _connection.Id;
            EndPoint = _connection.EndPoint;

            Log = new PersonalLog($"NetConnection ({Id} / {EndPoint})");
        }

        public void Disconnect()
        {
            if (!Connected)
            {
                Log.Warn($"Disconnect() called on an unconnected socket!");
                return;
            }

            _connection.Disconnect(true);
            _connection = null;
            _callbacks.Clear();
            _callbacks = null;

            Log.Debug($"Disposed!");
            Log = null;
        }

        public void AddCallback<T>(Action<T> callback) where T : INetMessage
        {
            _callbacks.Add(new NetCallback<T>()
                .WithIsTemporary(false)
                .WithCallback(callback));
        }

        public void AddTemporaryCallback<T>(Action<T> callback) where T : INetMessage
        {
            _callbacks.Add(new NetCallback<T>()
                .WithIsTemporary(true)
                .WithCallback(callback));
        }

        public void Send(NetPayload payload)
        {
            OnPayloadSending?.Invoke(payload);

            Send(payload.ToSegment());
        }

        public void Send(ArraySegment<byte> payload)
        {
            if (!Connected)
            {
                Log.Warn($"Send() called on an unconnected socket!");
                return;
            }

            OnDataSending?.Invoke(payload);

            _connection.Send(payload, 0, false, 0);
        }

        internal void Notify(ArraySegment<byte> payload)
        {
            if (!NetUtils.TryReadPayload(new ArraySegment<byte>(payload.Array, payload.Offset, payload.Count), out var netPayload))
            {
                Log.Warn($"Failed to read payload!");
                return;
            }

            OnPayloadProcessed?.Invoke(netPayload);

            foreach (var msg in netPayload.Messages)
            {
                Log.Debug($"{msg}");

                foreach (var callback in _callbacks)
                {
                    if (callback.IsValid(msg))
                    {
                        callback.Execute(msg);
                    }
                }

                _callbacks.RemoveWhere(x => x.IsTemporary && x.IsValid(msg));
            }
        }
    }
}
