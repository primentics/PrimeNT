﻿using System;
using System.Threading.Tasks;

using AzyWorks.Waiters.Exceptions;

namespace AzyWorks.Waiters.Features
{
    public class TaskDelayWaiter : IWait
    {
        public void Wait(Func<bool> condition, int timeout = -1)
        {
            Task.Run(async () =>
            {
                int curTime = 0;

                while (!condition())
                {
                    await Task.Delay(10);

                    if (timeout > 0)
                    {
                        curTime += 10;

                        if (curTime >= timeout)
                            throw new WaitTimedOutException(timeout);
                    }
                }
            });
        }

        public void WaitExecute(Func<bool> condition, Action execute, Action onTimeout = null, int timeout = -1)
        {
            Task.Run(async () =>
            {
                int curTime = 0;

                while (!condition())
                {
                    await Task.Delay(10);

                    if (timeout > 0)
                    {
                        curTime += 10;

                        if (curTime >= timeout)
                        {
                            onTimeout?.Invoke();

                            return;
                        }
                    }
                }

                execute?.Invoke();
            });
        }
    }
}