using AzyWorks.Timers;

using System;

namespace AzyWorks.Polling.Features.TimerBased
{
    public class TimerBasedPollableEvent
    {
        public int Interval { get; set; }
        public Action Action { get; set; }
        public Timer Timer { get; set; }
    }
}