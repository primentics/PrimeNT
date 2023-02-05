using System;

namespace AzyWorks.Polling
{
    public interface IPoller
    {
        void Start();

        void Poll(Action action, int interval);
        void Stop(Action action);
        void StopAll();

        void ManualPoll();
    }
}