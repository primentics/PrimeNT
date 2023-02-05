using AzyWorks.Utilities;

namespace AzyWorks.Networking
{
    public static class NetworkIdGenerator
    {
        public const int NetworkTicketSize = 20;

        public static NetworkId GenerateId(int cId)
        {
            return new NetworkId(cId, StaticRandom.RandomTicket(NetworkTicketSize));
        }
    }
}