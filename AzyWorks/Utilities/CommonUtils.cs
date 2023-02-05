using System;
using System.Collections.Generic;
using System.Net;

namespace AzyWorks.Utilities
{
    public static class CommonUtils
    {
        private static Random random = new Random();

        public const string ServiceAddress = "https://api.my-ip.io/ip.txt";

        public static IPAddress GetCurrentIpV4()
        {
            string ip = "";

            using (var client = new WebClient())
            {
                ip = client.DownloadString(ServiceAddress);
            }

            return IPAddress.TryParse(ip, out var ipAddress) ? ipAddress : null;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;

            while (n > 1)
            {
                n--;

                int k = random.Next(n + 1);

                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}