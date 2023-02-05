using SharpCompress.Compressors.Deflate;
using SharpCompress.Compressors;

using System.IO;

namespace AzyWorks.IO
{
    public static class Compression
    {
        public static byte[] CompressBytes(byte[] data, bool preferCompression)
        {
            byte[] buffer = null;

            using (var sourceStream = new MemoryStream(data))
            using (var destStream = new MemoryStream())
            using (var compressStream = new GZipStream(sourceStream, CompressionMode.Compress, preferCompression ? CompressionLevel.BestCompression : CompressionLevel.BestSpeed))
            {
                compressStream.Write(data, 0, data.Length);
                compressStream.Flush();
                compressStream.CopyTo(destStream);

                buffer = destStream.ToArray();
            }

            return buffer;
        }

        public static byte[] DecompressBytes(byte[] data)
        {
            byte[] buffer = null;

            using (var sourceStream = new MemoryStream(data))
            using (var destStream = new MemoryStream())
            using (var decompStream = new GZipStream(sourceStream, CompressionMode.Decompress))
            {
                decompStream.CopyTo(destStream);

                buffer = destStream.ToArray();
            }

            return buffer;
        }
    }
}