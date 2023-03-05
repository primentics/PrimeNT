using System;
using System.Collections.Generic;

namespace AzyWorks.Timers
{
    public static class StaticTimers
    {
        private static int _idIndex;
        private static Dictionary<string, Timer> _timers = new Dictionary<string, Timer>();

        public static string Start(
            double interval = 1000,
            Action<DateTime, double> onElapsed = null)
        {
            Timer timer = new Timer($"NLT{_idIndex++}", true, interval, onElapsed);

            _timers.Add(timer.Id, timer);

            return timer.Id;
        }

        public static void Stop(string id)
        {
            _timers[id].Stop();
        }

        public static void Reset(string id)
        {
            _timers[id].ResetTimer();
        }

        public static void Restart(string id)
        {
            _timers[id].Restart();
        }

        public static void Dispose(string id)
        {
            _timers[id].Stop();
            _timers.Remove(id);
        }

        public static double GetElapsedTime(string id)
        {
            return _timers[id].Elapsed;
        }

        public static void ClearTimers()
        {
            foreach (var timerKey in _timers.Keys)
                Dispose(timerKey);

            _timers.Clear();
        }

        public static Timer GetTimer(string id)
        {
            return _timers[id];
        }
    }
}
