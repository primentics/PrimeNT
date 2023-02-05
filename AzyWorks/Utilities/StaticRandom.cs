using System;
using System.Security.Cryptography;

namespace AzyWorks.Utilities
{
    public static class StaticRandom
    {
        private static Random _random = new Random();
        private static RandomNumberGenerator _rng = RandomNumberGenerator.Create();

        public static int RandomInt(int min, int max)
        {
            return _random.Next(min, max);
        }

        public static double RandomDouble()
        {
            return _random.NextDouble();
        }

        public static byte[] RandomBytes(int size)
        {
            byte[] bytes = new byte[size];

            _rng.GetBytes(bytes, 0, size);

            return bytes;
        }

        public static byte[] RandomBytes2(int size)
        {
            byte[] bytes = new byte[size];

            _random.NextBytes(bytes);

            return bytes;
        }

        public static byte[] RandomBytes2()
        {
            return RandomBytes2(RandomInt(16, 256));
        }

        public static byte[] RandomBytes()
        {
            return RandomBytes(RandomInt(16, 256));
        }

        public static byte[] RandomNonZeroBytes(int size)
        {
            byte[] bytes = new byte[size];

            _rng.GetNonZeroBytes(bytes);

            return bytes;
        }

        public static byte[] RandomNonZeroBytes()
        {
            return RandomNonZeroBytes(RandomInt(16, 256));
        }

        public static string RandomString(int size)
        {
            return BitConverter.ToString(RandomBytes(size));
        }

        public static string RandomString2(int size)
        {
            return BitConverter.ToString(RandomBytes2(size));
        }

        public static string RandomNonZeroString(int size)
        {
            return BitConverter.ToString(RandomNonZeroBytes(size));
        }

        public static string RandomString()
        {
            return BitConverter.ToString(RandomBytes());
        }

        public static string RandomString2()
        {
            return BitConverter.ToString(RandomBytes2());
        }

        public static string RandomNonZeroString()
        {
            return BitConverter.ToString(RandomNonZeroBytes());
        }

        public static string RandomIpV4Address()
        {
            return $"{RandomInt(1, 255)}.{RandomInt(1, 255)}.{RandomInt(1, 255)}.{RandomInt(1, 254)}";
        }

        public static string RandomTicket(int size)
        {
            return RandomNonZeroString(size).Replace("-", "").ToLowerInvariant();
        }

        public static bool RandomBoolean()
        {
            return RandomInt(0, 1) == 1;
        }
    }
}