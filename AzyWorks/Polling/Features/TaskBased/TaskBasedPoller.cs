using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzyWorks.Polling.Features.TaskBased
{
    public class TaskBasedPoller : IPoller
    {
        private List<TaskBasedPollableEvent> _events;

        public TaskBasedPoller()
        {
            _events = new List<TaskBasedPollableEvent>();
        }

        public void ManualPoll()
        {
            _events.ForEach(x => x.Action?.Invoke());
        }

        public void Poll(Action action, int interval)
        {
            _events.Add(new TaskBasedPollableEvent
            {
                Action = action,
                Interval = interval
            });
        }

        public void Start()
        {
            _events.ForEach(x =>
            {
                Task.Run(async () =>
                {
                    await Task.Delay(x.Interval);

                    x.Action?.Invoke();
                }, x.token);
            }); 
        }

        public void Stop(Action action)
        {
            var ev = _events.FirstOrDefault(x => x.Action == action);

            if (ev != null)
            {
                ev.source.Cancel();
            }
        }

        public void StopAll()
        {
            _events.ForEach(x => x.source.Cancel());
        }
    }
}