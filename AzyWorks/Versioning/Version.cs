using System;
using System.Text;
using System.Reflection;

namespace AzyWorks.Versioning
{
    public struct Version
    {
        public int Major { get; private set; }
        public int Minor { get; private set; }
        public int Revision { get; private set; }

        public char? Build { get; private set; }

        public Release Release { get; private set; }

        public string Signature
        {
            get
            {
                return BitConverter.ToString(Encoding.UTF32.GetBytes($"{ToString()}={Release.Signature}")).ToLowerInvariant().Replace("-", "");
            }
        }

        public override string ToString()
        {
            return $"v{Major}.{Minor}.{Revision}-{Release}";
        }

        public static Version Get(int major, int minor, int revision, char? build, Release release)
            => new Version
            {
                Major = major,
                Minor = minor,
                Revision = revision,
                Release = release,
                Build = build
            };

        public static Version Get(global::System.Version version, Release release)
            => new Version
            {
                Major = version.Major,
                Minor = version.Minor,
                Revision = version.Revision,
                Release = release
            };

        public static Version Get(Assembly assembly, Release release)
            => Get(assembly.GetName().Version, release);

        public static Version Get(Release release)
            => Get(Assembly.GetExecutingAssembly(), release);
    }
}