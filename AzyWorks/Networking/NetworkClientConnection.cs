using AzyWorks.Networking.Extensions;
using AzyWorks.Networking.Messages;

using LiteNetLib;
using LiteNetLib.Utils;

using System.Net;

namespace AzyWorks.Networking
{
    public class NetworkClientConnection
    {
        private NetPeer _peer;
        private NetworkServer _server;

        public NetworkId NetworkId;
        public NetworkRegistry Registry;

        public IPEndPoint EndPoint;

        public int Ping;

        public bool IsConnected;

        public NetworkClientConnection(NetPeer peer, NetworkServer server, NetworkId netId)
        {
            _peer = peer;
            _server = server;

            IsConnected = peer != null && peer.ConnectionState is ConnectionState.Connected;

            EndPoint = peer.EndPoint;
            NetworkId = netId;

            Registry = new NetworkRegistry(server, this);
        }

        public void Send(params INetworkMessage[] messages)
        {
            if (!IsConnected || _peer is null)
                return;

            if (messages.Length > 0)
            {
                var writer = new NetDataWriter(true);

                writer.WriteMessages(messages);

                _peer.Send(writer, DeliveryMethod.ReliableOrdered);
            }
        }

        public void Disconnect()
        {
            if (_peer != null)
            {
                _peer.Disconnect();
            }
        }
    }
}