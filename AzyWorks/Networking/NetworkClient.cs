using AzyWorks.Logging;
using AzyWorks.Networking.Extensions;
using AzyWorks.Networking.Messages;

using LiteNetLib;
using LiteNetLib.Utils;

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace AzyWorks.Networking
{
    public class NetworkClient
    {
        private Thread _connectThread;
        private PersonalLog log;

        private bool _hasAuthed;

        private NetPeer _peer;
        private NetManager _netManager;
        private EventBasedNetListener _netListener;

        public bool IsConnected;
        public bool ThreadSafe;

        public int Ping;

        public NetworkId ReceivedId;

        public NetworkRegistry Registry;
        public EndPoint EndPoint;
        public IPEndPoint DesiredEndPoint;

        public event Action<INetworkMessage> OnReceived;

        public event Action<SocketError> OnError;

        public event Action<NetworkId> OnAuthed;
        public event Action<DisconnectInfo> OnDisconnected;

        public event Action<IPEndPoint> OnConnected;
        public event Action<IPEndPoint> OnConnecting;

        public event Action OnStarted;
        public event Action OnStopped;

        public NetworkClient(IPEndPoint endPoint, bool threadSafe = false)
        {
            ThreadSafe = threadSafe;
            Registry = new NetworkRegistry(this);

            _netListener = new EventBasedNetListener();
            _netManager = new NetManager(_netListener);

            NetworkConfig.RunClientSetup(_netManager);

            _netListener.ConnectionRequestEvent += OnConnectionRequestedHandler;
            _netListener.PeerConnectedEvent += OnConnectedHandler;
            _netListener.PeerDisconnectedEvent += OnDisconnectedHandler;
            _netListener.NetworkErrorEvent += OnErrorHandler;
            _netListener.NetworkLatencyUpdateEvent += OnLatencyHandler;
            _netListener.NetworkReceiveEvent += OnDataHandler;

            _netManager.Start();

            DesiredEndPoint = endPoint;

            log = new PersonalLog($"Network Client ({endPoint.Port})");

            OnStarted?.Invoke();
        }

        public void Send(params INetworkMessage[] messages)
        {
            if (!_hasAuthed)
                return;

            var writer = new NetDataWriter(true);

            writer.WriteMessages(messages);

            if (_peer != null && IsConnected)
            {
                _peer.Send(writer, DeliveryMethod.ReliableOrdered);
            }
        }

        public void Connect()
        {
            if (!ThreadSafe)
                ConnectUnsafe();
            else
                ConnectSafe();
        }

        public void Disconnect()
        {
            if (_peer != null)
            {
                log.Info("Disconnecting ..");

                _peer.Disconnect();
            }
        }

        public void Tick()
            => _netManager.PollEvents();

        public void Dispose()
        {
            if (IsConnected)
                Disconnect();

            _netListener.ConnectionRequestEvent -= OnConnectionRequestedHandler;
            _netListener.PeerConnectedEvent -= OnConnectedHandler;
            _netListener.PeerDisconnectedEvent -= OnDisconnectedHandler;
            _netListener.NetworkErrorEvent -= OnErrorHandler;
            _netListener.NetworkLatencyUpdateEvent -= OnLatencyHandler;
            _netListener.NetworkReceiveEvent -= OnDataHandler;

            _hasAuthed = false;

            Registry.Dispose();
            Registry = null;

            _netListener = null;
            _netManager = null;
            _peer = null;

            OnStopped?.Invoke();
        }

        private void OnDataHandler(NetPeer peer, NetPacketReader reader, DeliveryMethod method)
        {
            if (method != DeliveryMethod.ReliableOrdered)
                return;

            try
            {
                var messages = reader.ReadMessages();

                if (messages.Length > 0)
                {
                    if (messages[0] is AuthSyncMessage authSync && !_hasAuthed)
                    {
                        ReceivedId = authSync.Id;

                        _hasAuthed = true;

                        log.Info($"Received authentification: {ReceivedId.ConnectionTicket} ({ReceivedId.ConnectionId}/{ReceivedId.ServerPeerId})");

                        OnAuthed?.Invoke(ReceivedId);
                    }

                    foreach (var msg in messages)
                    {
                        if (msg is AuthSyncMessage)
                            continue;

                        OnReceived?.Invoke(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Caught an exception when deserializing! {ex.Message}");
            }
        }

        private void OnLatencyHandler(NetPeer peer, int ping)
        {
            Ping = ping;
        }

        private void OnErrorHandler(IPEndPoint endpoint, SocketError error)
        { 
            OnError?.Invoke(error);

            log.Error($"Caught an error: {error} on {endpoint}");
        }

        private void OnDisconnectedHandler(NetPeer peer, DisconnectInfo info)
        {
            IsConnected = false;
            EndPoint = null;

            _peer = null;

            log.Info($"Disconnected! ({info.Reason})");

            OnDisconnected?.Invoke(info);
        }

        private void OnConnectedHandler(NetPeer peer)
        {
            _peer = peer;

            IsConnected = true;

            EndPoint = peer.EndPoint;

            log.Info($"Connected to {peer.EndPoint}!");

            OnConnected?.Invoke(_peer.EndPoint);
        }

        private void OnConnectionRequestedHandler(ConnectionRequest request)
        {
            log.Warn("Received a connection request on a client.");

            var writer = NetworkConstants.GetWriterForRejectReason("NOT_CLIENT");

            if (writer != null)
                request.Reject(writer);
            else
                request.Reject();
        }

        private void ConnectSafe()
        {
            if (IsConnected)
                Disconnect();

            OnConnecting?.Invoke(DesiredEndPoint);

            _connectThread = new Thread(() =>
            {
                _netManager.Connect(DesiredEndPoint, NetworkConfig.ClientConnectionRequest);
            });

            _connectThread.Start();
        }

        private void ConnectUnsafe()
        {
            if (IsConnected)
                Disconnect();

            OnConnecting?.Invoke(DesiredEndPoint);

            _netManager.Connect(DesiredEndPoint, NetworkConfig.ClientConnectionRequest);
        }
    }
}
