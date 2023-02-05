using AzyWorks.Networking.Messages;
using AzyWorks.Networking.Extensions;

using LiteNetLib;

using System;
using System.Linq;
using System.Net.Sockets;
using System.Net;

using AzyWorks.Logging;

namespace AzyWorks.Networking
{
    public class NetworkServer
    {
        private PersonalLog log;

        private NetManager _netManager;
        private EventBasedNetListener _netListener;

        public NetworkClientConnection[] Connections;

        public event Action<NetworkClientConnection, INetworkMessage> OnReceived;
        public event Action<NetworkClientConnection, SocketError> OnError;

        public event Action<NetworkClientConnection> OnDisconnected;
        public event Action<NetworkClientConnection> OnConnected;

        public event Action OnStarted;
        public event Action OnStopped;

        public NetworkServer(int port)
        {
            Connections = new NetworkClientConnection[NetworkConfig.MaxConnections];

            for (int i = 0; i < NetworkConfig.MaxConnections; i++)
            {
                Connections[i] = null;
            }

            _netListener = new EventBasedNetListener();
            _netManager = new NetManager(_netListener);

            NetworkConfig.RunServerSetup(_netManager);

            _netListener.ConnectionRequestEvent += OnConnectionRequestedHandler;
            _netListener.PeerConnectedEvent += OnConnectedHandler;
            _netListener.PeerDisconnectedEvent += OnDisconnectedHandler;
            _netListener.NetworkErrorEvent += OnErrorHandler;
            _netListener.NetworkLatencyUpdateEvent += OnLatencyHandler;
            _netListener.NetworkReceiveEvent += OnDataHandler;

            _netManager.Start(port);

            log = new PersonalLog($"Network Server ({port})");
            log.Info($"Server listening on port {port}");

            OnStarted?.Invoke();
        }

        public void Tick()
            => _netManager.PollEvents();

        public void Dispose()
        {
            foreach (var connection in Connections)
                connection.Disconnect();

            Array.Clear(Connections, 0, Connections.Length);

            _netListener.ConnectionRequestEvent -= OnConnectionRequestedHandler;
            _netListener.PeerConnectedEvent -= OnConnectedHandler;
            _netListener.PeerDisconnectedEvent -= OnDisconnectedHandler;
            _netListener.NetworkErrorEvent -= OnErrorHandler;
            _netListener.NetworkLatencyUpdateEvent -= OnLatencyHandler;
            _netListener.NetworkReceiveEvent -= OnDataHandler;

            _netListener = null;
            _netManager = null;

            OnStopped?.Invoke();
        }

        private void OnDataHandler(NetPeer peer, NetPacketReader reader, DeliveryMethod method)
        {
            if (method != DeliveryMethod.ReliableOrdered)
                return;

            var connection = GetConnectionOfPeer(peer);

            if (connection != null)
            {
                var messages = reader.ReadMessages();

                if (messages.Length > 0)
                {
                    foreach (var msg in messages)
                    {
                        if (msg is AuthSyncMessage)
                            continue;

                        OnReceived?.Invoke(connection, msg);
                    }
                }
            }
        }

        private void OnLatencyHandler(NetPeer peer, int ping)
        {
            var connection = GetConnectionOfPeer(peer);

            if (connection != null)
            {
                connection.Ping = ping;
            }
        }

        private void OnErrorHandler(IPEndPoint endpoint, SocketError error)
        {
            var connection = GetConnectionOfEndPoint(endpoint);

            if (connection != null)
            {
                OnError?.Invoke(connection, error);

                log.Warn($"Received a socket error for connection {connection.NetworkId.ConnectionTicket}: {error}");
            }
        }

        private void OnDisconnectedHandler(NetPeer peer, DisconnectInfo info)
        {
            var connection = GetConnectionOfPeer(peer);

            if (connection != null)
            {
                OnDisconnected?.Invoke(connection);

                Connections[connection.NetworkId.ConnectionId] = null;

                log.Info($"Client {peer.EndPoint} disconnected ({info.Reason}).");
            }
        }

        private void OnConnectedHandler(NetPeer peer)
        {
            var connectionId = GetNextId();
            var networkId = NetworkIdGenerator.GenerateId(connectionId);

            networkId.ServerPeerId = peer.Id;

            var connection = new NetworkClientConnection(peer, this, networkId);

            Connections[connectionId] = connection;

            connection.Send(new AuthSyncMessage(networkId));

            log.Info($"Client connected: {peer.EndPoint} ({networkId.ConnectionTicket})");

            OnConnected?.Invoke(connection);
        }

        private void OnConnectionRequestedHandler(ConnectionRequest request)
        {
            var version = request.Data.GetLong();

            if (version != NetworkConfig.NetworkVersion)
            {
                var rejectWriter = NetworkConstants.GetWriterForRejectReason("VERSION_MISMATCH");

                if (rejectWriter != null)
                {
                    request.Reject(rejectWriter);

                    log.Warn($"Rejected connection request from {request.RemoteEndPoint}: version mismatch.");
                }
            }
            else
            {
                request.Accept();

                log.Info($"Accepted connection request from {request.RemoteEndPoint}");
            }
        }

        private NetworkClientConnection GetConnectionOfPeer(NetPeer peer)
        {
            var connection = Connections.FirstOrDefault(x => x.NetworkId.ServerPeerId == peer.Id && x.EndPoint == peer.EndPoint);

            return connection;
        }

        private NetworkClientConnection GetConnectionOfEndPoint(IPEndPoint endPoint)
        {
            var connection = Connections.FirstOrDefault(x => x.EndPoint == endPoint);

            return connection;
        }

        private int GetNextId()
        {
            for (int i = 0; i < NetworkConfig.MaxConnections; i++)
            {
                if (Connections[i] is null)
                    return i;
            }

            return -1;
        }
    }
}
