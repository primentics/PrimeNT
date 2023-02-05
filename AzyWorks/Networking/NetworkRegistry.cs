using AzyWorks.Networking.ApiSync;
using AzyWorks.Networking.Messages;

using System;
using System.Collections.Generic;
using System.Linq;

namespace AzyWorks.Networking
{
    public class NetworkRegistry
    {
        private NetworkClient _client;
        private NetworkServer _server;
        private NetworkClientConnection _connection;

        private readonly Dictionary<Type, List<Action<INetworkMessage>>> _listeners = new Dictionary<Type, List<Action<INetworkMessage>>>();
        private readonly Dictionary<Type, List<Action<INetworkMessage>>> _tempListeners = new Dictionary<Type, List<Action<INetworkMessage>>>();
        private readonly HashSet<IApiStateBase> _apiSync = new HashSet<IApiStateBase>();

        public NetworkRegistry(NetworkClient client)
        {
            _client = client;
            _client.OnReceived += OnClientMessageReceived;
        }

        public NetworkRegistry(NetworkServer server, NetworkClientConnection c)
        {
            _server = server;
            _server.OnReceived += OnServerMessageReceived;
            _connection = c;
        }

        public void RegisterApi<TApiState>(TApiState instance) where TApiState : IApiStateBase
        {
            if (_client != null)
                instance.RegisterClientHandlers(this);

            if (_server != null && _connection != null)
                instance.RegisterServerHandlers(this);

            _apiSync.Add(instance);
        }

        public void AddHandler<T>(Action<T> handler) where T : INetworkMessage
        {
            var msgType = typeof(T);

            if (!_listeners.TryGetValue(msgType, out var list))
                _listeners[msgType] = (list = new List<Action<INetworkMessage>>());

            list.Add(x => handler?.Invoke((T)x));
        }

        public void AddTempHandler<T>(Action<T> handler) where T : INetworkMessage
        {
            var msgType = typeof(T);

            if (!_tempListeners.TryGetValue(msgType, out var list))
                _tempListeners[msgType] = (list = new List<Action<INetworkMessage>>());

            list.Add(x => handler?.Invoke((T)x));
        }

        public void Dispose()
        {
            if (_client != null)
            {
                _client.OnReceived -= OnClientMessageReceived;
                _client = null;
            }

            if (_server != null)
            {
                _server.OnReceived -= OnServerMessageReceived;
                _server = null;
            }

            _listeners.Clear();
            _tempListeners.Clear();
            _apiSync.Clear();
        }

        private void OnServerMessageReceived(NetworkClientConnection c, INetworkMessage message)
        {
            if (c.NetworkId.ConnectionTicket != _connection.NetworkId.ConnectionTicket)
                return;

            var type = message.GetType();

            if (_listeners.TryGetValue(type, out var listeners))
            {
                foreach (var listener in listeners)
                {
                    listener.Invoke(message);
                }
            }

            for (int i = 0; i < _tempListeners.Count; i++)
            {
                var pair = _tempListeners.ElementAt(i);

                if (pair.Key == type)
                {
                    foreach (var listener in pair.Value)
                    {
                        listener.Invoke(message);
                    }

                    _tempListeners.Remove(pair.Key);
                }
            }
        }

        private void OnClientMessageReceived(INetworkMessage message)
        {
            var type = message.GetType();

            if (_listeners.TryGetValue(type, out var listeners))
            {
                foreach (var listener in listeners)
                {
                    listener.Invoke(message);
                }
            }

            for (int i = 0; i < _tempListeners.Count; i++)
            {
                var pair = _tempListeners.ElementAt(i);

                if (pair.Key == type)
                {
                    foreach (var listener in pair.Value)
                    {
                        listener.Invoke(message);
                    }

                    _tempListeners.Remove(pair.Key);
                }
            }
        }
    }
}