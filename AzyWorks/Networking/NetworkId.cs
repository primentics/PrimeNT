namespace AzyWorks.Networking
{
    public struct NetworkId
    {
        public int ConnectionId;
        public int ServerPeerId;

        public string ConnectionTicket;

        public NetworkId(int cId, string cTicket, int pId = 0)
        {
            ConnectionId = cId;
            ConnectionTicket = cTicket;
            ServerPeerId = pId;
        }
    }
}