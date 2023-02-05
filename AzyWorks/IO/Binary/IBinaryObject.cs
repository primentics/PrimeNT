using System.IO;

namespace AzyWorks.IO.Binary
{
    public interface IBinaryObject
    {
        void Serialize(BinaryWriter writer);
        void Deserialize(BinaryReader reader);
    }
}