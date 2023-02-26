﻿using AzyWorks.Logging;
using AzyWorks.Timers;

using Ruffles.Connections;
using Ruffles.Core;

using System;
using System.Collections.Generic;
using System.Net;

namespace AzyWorks.Networking.Client
{
    public static class NetClient
    {
        private static Connection _connection;
        private static RuffleSocket _socket;
        private static Timer _pollTimer;

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
                    ConnectionTimeout = 1000,
                    HeartbeatDelay = 500,
                    ConnectionRequestTimeout = 1500,
                    ConnectionChallengeTimeWindow = 15
                });

                _pollTimer = new Timer("netclient", false, 100, (x, y) => Poll());
                _socket.Start();
                _pollTimer.Start();

                IsActive = true;

                OnStarted?.Invoke();

                Log.Info("Socket started.");

                _socket.Connect(Config.ServerEndpoint);
            }
            catch (Exception ex)
            {
                Log.Error($"Start() failed: {ex}");
            }
        }

        public static void Stop()
        {
            _socket.Stop();
            _pollTimer.Stop();
            _pollTimer.ResetTimer();
            _callbacks.Clear();
            _connection = null;

            IsActive = false;

            OnStopped?.Invoke();
        }

        public static void Dispose()
        {
            Stop();

            _socket.Shutdown();
            _socket = null;
            _pollTimer.Dispose();
            _pollTimer = null;
            _callbacks = null;

            OnDisposed?.Invoke();

            Log = null;
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

                        OnConnected?.Invoke();
                    }
                    else if (ev.Type is NetworkEventType.Disconnect || ev.Type is NetworkEventType.Timeout)
                    {
                        _connection = null;

                        OnDisconnected?.Invoke();
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
    }
}