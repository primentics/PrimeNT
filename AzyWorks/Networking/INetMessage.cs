using System.IO;

namespace AzyWorks.Networking
{
    public interface INetMessage
    {
        void Serialize(BinaryWriter writer);
        void Deserialize(BinaryReader reader);  
    }
}