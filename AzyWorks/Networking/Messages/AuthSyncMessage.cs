using LiteNetLib.Utils;

namespace AzyWorks.Networking.Messages
{
    public struct AuthSyncMessage : INetworkMessage
    {
        public NetworkId Id;

        public AuthSyncMessage(NetworkId id)
            => Id = id;

        public void Deserialize(NetDataReader reader)
        {
            Id = new NetworkId(reader.GetInt(), reader.GetString(), reader.GetInt());
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id.ConnectionId);
            writer.Put(Id.ConnectionTicket);
            writer.Put(Id.ServerPeerId);
        }
    }
}
