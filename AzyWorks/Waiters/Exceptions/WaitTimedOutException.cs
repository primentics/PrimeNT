using System;

namespace AzyWorks.Waiters.Exceptions
{
    /// <summary>
    /// Gets thrown when the waiter times out.
    /// </summary>
    public class WaitTimedOutException : Exception
    {
        public WaitTimedOutException(int timeout) : base($"Timed out! (waited for {timeout} ms).") { }
    }
}