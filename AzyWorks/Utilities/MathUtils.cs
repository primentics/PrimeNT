namespace AzyWorks.Utilities
{
    public static class MathUtils
    {
        public static long GetByteArraySize(byte[] array)
        {
            long total = 0;

            foreach (byte b in array)
            {
                total += b;
            }

            return total;
        }
    }
}