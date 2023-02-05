using System;

namespace AzyWorks.Utilities
{
    public static class ThrowHelper
    {
        public static void Throw<T>() where T : Exception, new()
        {
            throw new T();
        }

        public static void LogAndThrow<T>() where T : Exception, new()
        {
            Exception ex = new T();

            Log.SendError("ExceptionManager", ex);

            throw ex;
        }

        public static void Throw(object message)
        {
            throw new Exception(message.ToString());
        }

        public static void LogAndThrow(object message)
        {
            Exception ex = new Exception(message.ToString());

            Log.SendError("ExceptionManager", ex);

            throw ex;
        }
    }
}
