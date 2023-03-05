using System;
using System.Timers;

namespace AzyWorks.Timers
{
    public class Timer 
    {
        private double _interval;
        private global::System.Timers.Timer _timer;
        private Action<DateTime, double> _onElapsed;
        private double _elapsed;
        private string _id;

        public bool IsEnabled { get => _timer.Enabled; set => _timer.Enabled = value; }
        public double Elapsed { get => _elapsed; }
        public string Id { get => _id; }

        public Timer(
            string id, 
            bool autoStart = false, 
            double interval = 1000, 
            Action<DateTime, double> onElapsed = null)
        {
            _id = id;
            _onElapsed = onElapsed;
            _interval = interval;
            _timer = new global::System.Timers.Timer(_interval);
            _timer.Elapsed += OnTimerElapsed;

            if (autoStart)
                Start();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _elapsed += _interval;
            _onElapsed?.Invoke(e.SignalTime, _elapsed);
        }

        public void Start()
        {
            if (_timer != null)
            {
                if (!_timer.Enabled)
                {
                    _timer.Enabled = true;
                }
            }
        }

        public void Stop()
        {
            if (_timer != null)
            {
                if (_timer.Enabled)
                {
                    _timer.Enabled = false;
                }
            }
        }

        public void ResetTimer()
        {
            _elapsed = 0;
        }

        public void Restart()
        {
            Stop();
            ResetTimer();
            Start();
        }
    }
}
