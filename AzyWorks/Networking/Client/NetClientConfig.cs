using System.Net;

namespace AzyWorks.Networking.Client
{
    public class NetClientConfig
    {
        private IPEndPoint _endPoint;

        public int ReconnectAttempts { get; set; } = 5;
        public int ReconnectTimeout { get; set; } = 1500;

        public int Port { get; set; } = 8888;

        public IPEndPoint ServerEndpoint
        {
            get => _endPoint is null ? (_endPoint = new IPEndPoint(IPAddress.Loopback, Port)) : _endPoint;
            set => _endPoint = value;
        }
    }
}
