using System;

namespace AzyWorks.Waiters
{
    /// <summary>
    /// Creates a blocking task/thread/coroutine, etc.
    /// </summary>
    public interface IWait
    {
        /// <summary>
        /// Waits for the specified amount of time.
        /// </summary>
        /// <param name="condition">The condition to check if we should stop waiting.</param>
        /// <param name="timeout">The maximum timeout.</param>
        void Wait(Func<bool> condition, int timeout = -1);

        /// <summary>
        /// Waits for the specified amount of time and executes the action.
        /// </summary>
        /// <param name="condition">The condition to check if we should stop waiting.</param>
        /// <param name="execute">The method to execute.</param>
        /// <param name="onTimeout">The method to execute if the wait times out.</param>
        /// <param name="timeout">The maximum timeout.</param>
        void WaitExecute(Func<bool> condition, Action execute, Action onTimeout = null, int timeout = -1);
    }
}