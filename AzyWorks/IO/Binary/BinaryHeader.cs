using AzyWorks.Versioning;

using System.IO;

namespace AzyWorks.IO.Binary
{
    public class BinaryHeader : IBinaryObject
    {
        public static readonly Version Current = Version.Get(1, 0, 1, 'A', Release.ProductionRelease);

        public Version Version { get; set; } = Current;

        public BinaryObject[] Objects { get; set; }

        public void Deserialize(BinaryReader reader)
        {
            Version = Version.Get(
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(), 

                reader.ReadChar(),
                
                Release.ProductionRelease);

            if (Version.Signature != Current.Signature)
                throw new InvalidDataException($"File version mismatch!");

            var size = reader.ReadInt32();

            Objects = new BinaryObject[size];

            for (int i = 0; i < size; i++)
            {
                var obj = new BinaryObject();  

                var objSize = reader.ReadInt32();
                var objData = reader.ReadBytes(objSize);

                obj.Data = objData;
                obj.QualifiedName = reader.ReadString();

                Objects[i] = obj;
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Version.Major);
            writer.Write(Version.Minor);
            writer.Write(Version.Revision);
            writer.Write(Version.Build.Value);

            writer.Write(Objects.Length);

            for (int i = 0; i < Objects.Length; i++)
            {
                var obj = Objects[i];

                writer.Write(obj.Data.Length);
                writer.Write(obj.Data);
                writer.Write(obj.QualifiedName);
            }
        }
    }
}