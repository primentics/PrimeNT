using AzyWorks.Properties;

namespace AzyWorks
{
    /// <summary>
    /// Used to load the library.
    /// </summary>
    public static class LibProperties
    {
        /// <summary>
        /// Gets the library version.
        /// </summary>
        /// <value>
        /// The library version.
        /// </value>
        public static Versioning.Version Version { get; }

        /// <summary>
        /// Whether or not to use native sockets.
        /// </summary>
        public static bool NetUdpApi_UseNativeSockets = true;

        /// <summary>
        /// Whether or not to include an assembly name when using <see cref="System.Type"/> as the source in <see cref="Logging.Features.MessageSource"/>
        /// </summary>
        public static bool LogApi_MessageSource_TypeSource_IncludeAssembly = false;

        static LibProperties()
        {
            string[] versArgs = Resources.LibraryVersion.Split('-');

            Version = Versioning.Version.Get(
                int.Parse(versArgs[0]),
                int.Parse(versArgs[1]),
                int.Parse(versArgs[2]),
                versArgs[3][0],
                Versioning.Release.ProductionRelease);
        }
    }
}