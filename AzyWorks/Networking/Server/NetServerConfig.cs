using System.Net;

namespace AzyWorks.Networking.Server
{
    public class NetServerConfig
    {
        private IPEndPoint _endPoint;

        public int Port { get; set; } = 8888;

        public IPEndPoint ListeningEndPoint
        {
            get => _endPoint is null ? (_endPoint = new IPEndPoint(IPAddress.Any, Port)) : _endPoint;
            set => _endPoint = value;
        }
    }
}