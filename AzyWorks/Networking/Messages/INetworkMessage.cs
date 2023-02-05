using LiteNetLib.Utils;

namespace AzyWorks.Networking.Messages
{
    public interface INetworkMessage
    {
        void Serialize(NetDataWriter writer);
        void Deserialize(NetDataReader reader);
    }
}