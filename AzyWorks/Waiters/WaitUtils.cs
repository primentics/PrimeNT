using AzyWorks.Waiters.Features;

using System;

namespace AzyWorks.Waiters
{
    public static class WaitUtils
    {
        public static IWait WhileLoop()
        {
            return new WhileLoopWaiter();
        }

        public static IWait ThreadSleep()
        {
            return new ThreadSleepWaiter();
        }

        public static IWait TaskDelay()
        {
            return new TaskDelayWaiter();
        }

        public static IWait SpinWaiter()
        {
            return new SpinWaiter();
        }

        public static void Wait(IWait waiter, Func<bool> condition, int timeout)
        {
            waiter.Wait(condition, timeout);
        }

        public static void Wait(IWait waiter, Func<bool> condition, Action execute, Action onTimeout = null, int timeout = -1)
        {
            waiter.WaitExecute(condition, execute, onTimeout, timeout);
        }
    }
}