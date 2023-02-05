using System;
using System.Threading;

using AzyWorks.Waiters.Exceptions;

namespace AzyWorks.Waiters.Features
{
    public class ThreadSleepWaiter : IWait
    {
        public void Wait(Func<bool> condition, int timeout = -1)
        {
            int curTime = 0;

            while (!condition())
            {
                Thread.Sleep(10);

                curTime += 10;

                if (curTime >= timeout)
                    throw new WaitTimedOutException(timeout);
            }
        }

        public void WaitExecute(Func<bool> condition, Action execute, Action onTimeout = null, int timeout = -1)
        {
            int curTime = 0;

            while (!condition())
            {
                Thread.Sleep(10);

                curTime += 10;

                if (curTime >= timeout)
                {
                    onTimeout?.Invoke();

                    return;
                }
            }

            execute?.Invoke();
        }
    }
}
