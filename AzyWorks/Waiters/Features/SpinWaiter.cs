using AzyWorks.Waiters.Exceptions;

using System;
using System.Threading;

namespace AzyWorks.Waiters.Features
{
    public class SpinWaiter : IWait
    {
        private SpinWait _waiter;

        public SpinWaiter()
        {
            _waiter = new SpinWait();
        }

        public void Wait(Func<bool> condition, int timeout = -1)
        {
            while (!condition())
            {
                _waiter.SpinOnce();

                if (_waiter.Count >= timeout)
                {
                    _waiter.Reset();

                    throw new WaitTimedOutException(timeout);
                }
            }
        }

        public void WaitExecute(Func<bool> condition, Action execute, Action onTimeout = null, int timeout = -1)
        {
            while (!condition())
            {
                _waiter.SpinOnce();

                if (_waiter.Count >= timeout)
                {
                    _waiter.Reset();

                    onTimeout?.Invoke();

                    return;
                }
            }

            execute?.Invoke();
        }
    }
}
