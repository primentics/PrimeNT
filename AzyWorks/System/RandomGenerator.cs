using System;
using System.Security.Cryptography;

namespace AzyWorks.System
{
    public static class RandomGenerator
    {
        private static Random _random = new Random();
        private static RandomNumberGenerator _rng = RandomNumberGenerator.Create();

        public static int Int32(int min, int max)
        {
            return _random.Next(min, max);
        }

        public static double Double()
        {
            return _random.NextDouble();
        }

        public static byte[] CryptoBytes(int size)
        {
            byte[] bytes = new byte[size];

            _rng.GetBytes(bytes, 0, size);

            return bytes;
        }

        public static byte[] Bytes(int size)
        {
            byte[] bytes = new byte[size];

            _random.NextBytes(bytes);

            return bytes;
        }

        public static byte[] Bytes()
        {
            return Bytes(Int32(byte.MinValue, byte.MaxValue));
        }

        public static byte[] CryptoBytes()
        {
            return CryptoBytes(Int32(byte.MinValue, byte.MaxValue));
        }

        public static byte[] CryptoNonZeroBytes(int size)
        {
            byte[] bytes = new byte[size];

            _rng.GetNonZeroBytes(bytes);

            return bytes;
        }

        public static string String(int size)
        {
            return BitConverter.ToString(Bytes(size));
        }

        public static string CryptoString(int size)
        {
            return BitConverter.ToString(CryptoBytes(size));
        }

        public static string CryptoNonZeroString(int size)
        {
            return BitConverter.ToString(CryptoNonZeroBytes(size));
        }

        public static string String()
        {
            return BitConverter.ToString(Bytes());
        }

        public static string CryptoString()
        {
            return BitConverter.ToString(CryptoBytes());
        }

        public static string CryptoNonZeroString()
        {
            return BitConverter.ToString(CryptoNonZeroBytes(Int32(byte.MinValue, byte.MaxValue)));
        }

        public static string RandomIpV4Address()
        {
            return $"{Int32(1, 255)}.{Int32(1, 255)}.{Int32(1, 255)}.{Int32(1, 254)}";
        }

        public static string Ticket(int size)
        {
            return CryptoNonZeroString(size).Replace("-", "").ToLowerInvariant();
        }

        public static bool Boolean()
        {
            return Int32(0, 1) == 1;
        }
    }
}