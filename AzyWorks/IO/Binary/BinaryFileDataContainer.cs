using System.Collections.Generic;
using System.IO;

namespace AzyWorks.IO.Binary
{
    public class BinaryFileDataContainer : IBinaryObject
    {
        private Dictionary<string, BinaryObject> Container = new Dictionary<string, BinaryObject>();

        public void Deserialize(BinaryReader reader)
        {
            Clear();

            var count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                var key = reader.ReadString();
                var type = reader.ReadString();
                var value = reader.ReadBytes(reader.ReadInt32());

                TryInsertData(key, new BinaryObject()
                {
                    Data = value,
                    QualifiedName = type
                });
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Container.Count);

            foreach (var pair in Container)
            {
                writer.Write(pair.Key);
                writer.Write(pair.Value.QualifiedName);
                writer.Write(pair.Value.Data.Length);
                writer.Write(pair.Value.Data);
            }
        }

        public bool TryGetData(string key, out BinaryObject binaryObject)
            => Container.TryGetValue(key, out binaryObject);

        public bool TryInsertData(string key, BinaryObject binaryObject)
        {
            try
            {
                Container[key] = binaryObject;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Clear()
        {
            Container.Clear();
        }
    }
}
