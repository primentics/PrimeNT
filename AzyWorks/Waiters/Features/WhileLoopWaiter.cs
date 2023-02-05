using System;

namespace AzyWorks.Waiters.Features
{
    public class WhileLoopWaiter : IWait
    {
        public void Wait(Func<bool> condition, int timeout = -1)
        {
            while (!condition())
                continue;
        }

        public void WaitExecute(Func<bool> condition, Action execute, Action onTimeout = null, int timeout = -1)
        {
            while (!condition())
                continue;

            execute?.Invoke();
        }
    }
}