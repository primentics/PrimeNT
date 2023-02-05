using System;
using System.Threading;

namespace AzyWorks.Polling.Features.TaskBased
{
    public class TaskBasedPollableEvent
    {
        internal CancellationToken token;
        internal CancellationTokenSource source;

        public TaskBasedPollableEvent()
        {
            source = new CancellationTokenSource();
            token = source.Token;
        }

        public Action Action { get; set; }
        public int Interval { get; set; }
    }
}