using LiteNetLib;
using LiteNetLib.Utils;

namespace AzyWorks.Networking
{
    public static class NetworkConfig
    {
        public const long NetworkVersion = 030220232220;

        public static int PingIntervalMs = 10;
        public static int ReconnectDelayMs = 1000;
        public static int UpdateTimeMs = 100;

        public static int MaxConnections = 100;

        public static readonly NetDataWriter ClientConnectionRequest;

        static NetworkConfig()
        {
            ClientConnectionRequest = new NetDataWriter();
            ClientConnectionRequest.Put(NetworkVersion);
        }

        public static void RunClientSetup(NetManager net)
        {
            net.PingInterval = PingIntervalMs;
            net.ReconnectDelay = ReconnectDelayMs;
            net.UnconnectedMessagesEnabled = false;
            net.UpdateTime = UpdateTimeMs;
        }

        public static void RunServerSetup(NetManager net) 
        {
            net.PingInterval = PingIntervalMs;
            net.ReconnectDelay = ReconnectDelayMs;
            net.ReuseAddress = true;
            net.UnconnectedMessagesEnabled = false;
            net.UpdateTime = UpdateTimeMs;
        }
    }
}
