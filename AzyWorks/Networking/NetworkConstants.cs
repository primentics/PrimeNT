using LiteNetLib.Utils;

namespace AzyWorks.Networking
{
    public static class NetworkConstants
    {
        public static NetDataWriter GetWriterForRejectReason(string reason)
        {
            switch (reason)
            {
                case "NOT_SERVER":
                    {
                        var writer = new NetDataWriter(true);

                        writer.Put(0);

                        return writer;
                    }

                case "VERSION_MISMATCH":
                    {
                        var writer = new NetDataWriter(true);

                        writer.Put(1);
                        writer.Put(NetworkConfig.NetworkVersion);

                        return writer;
                    }

                default:
                    return null;
            }
        }
    }
}
