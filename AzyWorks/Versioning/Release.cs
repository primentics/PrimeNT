using System;
using System.Text;

namespace AzyWorks.Versioning
{
    public struct Release
    {
        public ReleaseType Type { get; private set; }

        public string Name { get; private set; }

        public string Signature
        {
            get
            {
                return BitConverter.ToString(Encoding.UTF32.GetBytes(Name)).ToLowerInvariant().Replace("-", "");
            }
        }

        public static Release Get(string name, ReleaseType type)
            => new Release
            {
                Name = name,
                Type = type
            };

        public override string ToString()
        {
            return Name.ToLower();
        }

        public static readonly Release TestingRelease = Get("Testing", ReleaseType.InTesting);
        public static readonly Release BetaRelease = Get("Beta", ReleaseType.BetaVersion);
        public static readonly Release DevelopmentRelease = Get("Development", ReleaseType.InDevelopment);
        public static readonly Release ProductionRelease = Get("Production", ReleaseType.ForProduction);
    }
}