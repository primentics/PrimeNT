using AzyWorks.Logging;
using AzyWorks.Timers;

using Ruffles.Core;

using System;
using System.Collections.Generic;

namespace AzyWorks.Networking.Server
{
    public static class NetServer
    {
        private static RuffleSocket _socket;
        private static Timer _pollTimer;

        private static Dictionary<ulong, NetConnection> _connections;

        private static PersonalLog Log;

        public static bool IsActive { get; private set; }

        public static NetServerConfig Config { get; } = new NetServerConfig();

        public static IReadOnlyDictionary<ulong, NetConnection> Connections { get => _connections; }

        public static event Action OnStarted;
        public static event Action OnStopped;
        public static event Action OnDisposed;

        public static event Action<NetConnection> OnConnected;
        public static event Action<NetConnection> OnDisconnected;
        public static event Action<NetConnection, ArraySegment<byte>> OnReceived;

        static NetServer()
        {
            Log = new PersonalLog("NetServer");
        }

        public static void Start()
        {
            try
            {
                Log.Debug("Starting ..");

                _connections = new Dictionary<ulong, NetConnection>();

                _socket = new RuffleSocket(new Ruffles.Configuration.SocketConfig
                {
                    UseIPv6Dual = false,
                    HandshakeTimeout = 2500,
                    ConnectionTimeout = 1000,
                    HeartbeatDelay = 500,
                    ConnectionRequestTimeout = 1500,
                    ConnectionChallengeTimeWindow = 15,
                    DualListenPort = Config.Port,
                    ChannelTypes = new Ruffles.Channeling.ChannelType[] { Ruffles.Channeling.ChannelType.ReliableOrdered }
                });

                _pollTimer = new Timer("netserver", false, 100, (x, y) => Poll());
                _socket.Start();
                _pollTimer.Start();

                IsActive = true;

                OnStarted?.Invoke();

                Log.Info("Listening socket started.");
            }
            catch (Exception ex)
            {
                Log.Error($"Start() failed: {ex}");
            }
        }

        public static void Stop()
        {
            _socket.Stop();
            _connections.Clear();
            _pollTimer.Stop();
            _pollTimer.ResetTimer();

            IsActive = false;

            OnStopped?.Invoke();
        }

        public static void Dispose()
        {
            Stop();

            _socket.Shutdown();
            _socket = null;
            _connections = null;
            _pollTimer.Dispose();
            _pollTimer = null;

            OnDisposed?.Invoke();

            Log = null;
        }

        private static void Poll()
        {
            try
            {
                var ev = _socket.Poll();

                if (ev.Type != NetworkEventType.Nothing)
                {
                    if (ev.Type is NetworkEventType.Connect)
                    {
                        try
                        {
                            var netConnection = new NetConnection(ev.Connection);

                            OnConnected?.Invoke(netConnection);

                            _connections[ev.Connection.Id] = netConnection;

                            Log.Info($"Client {ev.Connection.Id} ({ev.Connection.EndPoint}) has connected.");
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"ConnectClient() failed: {ex}");
                        }
                    }
                    else if (ev.Type is NetworkEventType.Disconnect || ev.Type is NetworkEventType.Timeout)
                    {
                        try
                        {
                            if (_connections.TryGetValue(ev.Connection.Id, out var netConnection) && netConnection.Connected)
                                netConnection.Disconnect();

                            _connections.Remove(ev.Connection.Id);

                            OnDisconnected?.Invoke(netConnection);

                            Log.Info($"Client {ev.Connection.Id} ({ev.Connection.EndPoint}) has disconnected.");
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"DisconnectClient() failed: {ex}");
                        }
                    }
                    else if (ev.Type is NetworkEventType.Data)
                    {
                        try
                        {
                            if (!_connections.TryGetValue(ev.Connection.Id, out var netConnection)
                                || netConnection is null)
                            {
                                Log.Warn($"ReceiveData() failed to retrieve connection {ev.Connection.Id} ({ev.Connection.EndPoint})");
                                return;
                            }

                            OnReceived?.Invoke(netConnection, ev.Data);

                            netConnection.Notify(ev.Data);
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"ReceiveData() failed: {ex}");
                        }
                    }
                }

                ev.Recycle();
            }
            catch (Exception ex)
            {
                Log.Error($"Poll() failed: {ex}");
            }
        }
    }
}