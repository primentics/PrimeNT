using AzyWorks.Polling.Features.TimerBased;
using AzyWorks.Polling.Features.TaskBased;

namespace AzyWorks.Polling
{
    public static class PollUtils
    {
        public static IPoller TaskBasedPoller()
        {
            return new TaskBasedPoller();
        }

        public static IPoller TimerBasedPoller()
        {
            return new TimerBasedPoller();
        }
    }
}