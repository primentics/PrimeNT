using AzyWorks.Utilities;

using System;

namespace AzyWorks.Encryption
{
    public static class SimpleEncryption
    {
        public static int[] GenerateKey(int size)
        {
            var key = new int[size];
            
            for (int i = 0; i < size; i++)
            {
                key[i] = i;
            }

            key.Shuffle();

            return key;
        }

        public static byte[] Encrypt(byte[] originalData, int[] key)
        {
            if (originalData.Length > key.Length)
                throw new InvalidOperationException($"The key has to be long enough!");

            var encryptedData = new byte[originalData.Length];
           
            for (int i = 0; i < encryptedData.Length; i++)
            {
                encryptedData[i] = originalData[key[i]];
            }

            return encryptedData;
        }

        public static byte[] Decrypt(byte[] encryptedData, int[] key)
        {
            if (encryptedData.Length > key.Length)
                throw new InvalidOperationException("The key has to be long enough");

            var originalData = new byte[encryptedData.Length];

            for (int i = 0; i < encryptedData.Length; i++)
            {
                originalData[key[i]] = encryptedData[i];
            }

            return originalData;
        }
    }
}