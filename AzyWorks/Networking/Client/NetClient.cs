using AzyWorks.Logging;
using AzyWorks.Timers;

using Ruffles.Connections;
using Ruffles.Core;

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace AzyWorks.Networking.Client
{
    public static class NetClient
    {
        private static Connection _connection;
        private static RuffleSocket _socket;
        private static Timer _pollTimer;

        private static int _recAtt;

        private static HashSet<NetCallbackBase> _callbacks;

        private static PersonalLog Log;

        public static bool IsActive { get; private set; }
        public static bool IsConnected { get => _connection?.State is ConnectionState.Connected; }
        public static bool IsActiveAndConnected { get => IsActive && IsConnected; }

        public static int Ping { get => _connection?.Roundtrip ?? -1; }

        public static IPEndPoint EndPoint { get => _connection?.EndPoint; }

        public static NetClientConfig Config { get; } = new NetClientConfig();

        public static event Action OnStarted;
        public static event Action OnStopped;
        public static event Action OnDisposed;

        public static event Action OnConnected;
        public static event Action OnDisconnected;

        public static event Action<ArraySegment<byte>> OnReceived;
        public static event Action<ArraySegment<byte>> OnSending;

        public static event Action<NetPayload> OnPayloadReceived;
        public static event Action<NetPayload> OnPayloadSending;

        static NetClient()
        {
            Log = new PersonalLog($"NetClient");
        }

        public static void Start()
        {
            try
            {
                Log.Debug("Starting ..");

                _callbacks = new HashSet<NetCallbackBase>();

                _socket = new RuffleSocket(new Ruffles.Configuration.SocketConfig
                {
                    UseIPv6Dual = false,
                    HandshakeTimeout = 2500,
                    ConnectionTimeout = 2500,
                    HeartbeatDelay = 50,
                    ConnectionRequestTimeout = 1500,
                    ConnectionChallengeTimeWindow = 15
                });

                _pollTimer = new Timer("netclient", false, 100, (x, y) => Poll());
                _socket.Start();
                _pollTimer.Start();

                IsActive = true;

                OnStarted?.Invoke();

                Log.Info("Socket started.");

                Connect();
            }
            catch (Exception ex)
            {
                Log.Error($"Start() failed: {ex}");
            }
        }

        public static void Connect()
        {
            if (IsConnected)
                Disconnect();

            _socket.Connect(Config.ServerEndpoint);
        }

        public static void Disconnect()
        {
            _connection?.Disconnect(true);
        }

        public static void Stop()
        {
            Disconnect();

            _socket.Shutdown();
            _pollTimer.Stop();
            _callbacks.Clear();

            IsActive = false;

            OnStopped?.Invoke();
        }

        public static void Dispose()
        {
            _socket = null;
            _pollTimer = null;
            _callbacks = null;

            OnDisposed?.Invoke();
        }

        public static void Send(NetPayload payload)
        {
            OnPayloadSending?.Invoke(payload);

            Send(payload.ToSegment());
        }

        public static void Send(ArraySegment<byte> data)
        {
            if (!IsActiveAndConnected)
                return;

            OnSending?.Invoke(data);

            _connection.Send(data, 0, false, 0);
        }

        public static void AddCallback<T>(Action<T> callback) where T : INetMessage
        {
            _callbacks.Add(new NetCallback<T>()
                .WithIsTemporary(false)
                .WithCallback(callback));
        }

        public static void AddTemporaryCallback<T>(Action<T> callback) where T : INetMessage
        {
            _callbacks.Add(new NetCallback<T>()
                .WithIsTemporary(true)
                .WithCallback(callback));
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
                        _connection = ev.Connection;
                        _recAtt = 0;

                        OnConnected?.Invoke();
                    }
                    else if (ev.Type is NetworkEventType.Disconnect || ev.Type is NetworkEventType.Timeout)
                    {
                        _connection = null;

                        OnDisconnected?.Invoke();

                        if (Config.ReconnectAttempts > 0 && _recAtt < Config.ReconnectAttempts)
                        {
                            Task.Run(async () => await ReconnectAsync());

                            _recAtt++;
                        }
                    }
                    else if (ev.Type is NetworkEventType.Data)
                    {
                        OnReceived?.Invoke(ev.Data);

                        if (!NetUtils.TryReadPayload(new ArraySegment<byte>(ev.Data.Array, ev.Data.Offset, ev.Data.Count), out var payload))
                            return;

                        OnPayloadReceived?.Invoke(payload);

                        foreach (var msg in payload.Messages)
                        {
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

                ev.Recycle();
            }
            catch (Exception ex)
            {
                Log.Error($"Poll() failed: {ex}");
            }
        }

        private static async Task ReconnectAsync()
        {
            if (_recAtt >= Config.ReconnectAttempts)
                return;

            await Task.Delay(Config.ReconnectTimeout);

            Connect();
        }
    }
}
