using System;
using System.Collections.Generic;
using System.Linq;

namespace AzyWorks.Polling.Features.TimerBased
{
    public class TimerBasedPoller : IPoller
    {
        private List<TimerBasedPollableEvent> _events;

        public TimerBasedPoller()
        {
            _events = new List<TimerBasedPollableEvent>();
        }

        public void ManualPoll()
        {
            _events.ForEach(x => x.Action?.Invoke());
        }

        public void Poll(Action action, int interval)
        {
            _events.Add(new TimerBasedPollableEvent
            {
                Action = action,
                Interval = interval,
                Timer = new Timers.Timer(GetHashCode().ToString(), false, interval, (x, y) => action?.Invoke())
            });
        }

        public void Start()
        {
            _events.ForEach(x => x.Timer.Start());
        }

        public void Stop(Action action)
        {
            var timer = _events.FirstOrDefault(x => x.Action == action)?.Timer;

            if (timer != null)
            {
                timer.Stop();
            }
        }

        public void StopAll()
        {
            _events.ForEach(x => x.Timer.Stop());
        }
    }
}