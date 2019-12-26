using System;
using System.Threading.Tasks;
using System.Timers;

namespace Orikivo
{
    public class AsyncTimer
    {
        private bool _started = false;

        public AsyncTimer(TimeSpan? duration)
        {
            InternalTimer = new Timer
            {
                Enabled = false
            };

            InternalTimer.AutoReset = false;
            Timeout = duration;
            TimeStarted = Signal = null;
            CompletionSource = new TaskCompletionSource<bool>();
            InternalTimer.Elapsed += OnElapse;
            Elapsed = false;
        }

        private Timer InternalTimer { get; }

        private DateTime? TimeStarted { get; set; }
        private DateTime? Signal { get; set; }

        public bool Elapsed { get; private set; }

        public TimeSpan ElapsedTime => TimeStarted.HasValue ? DateTime.UtcNow - TimeStarted.Value : Signal.HasValue ? Signal.Value - TimeStarted.Value : TimeSpan.Zero;

        public TaskCompletionSource<bool> CompletionSource { get; private set; }

        public TimeSpan? Timeout
        {
            get => TimeSpan.FromMilliseconds(InternalTimer.Interval);
            set
            {
                if (value.HasValue)
                {
                    InternalTimer.Interval = value.Value.TotalMilliseconds;
                    InternalTimer.Enabled = true;
                }
                else
                {
                    InternalTimer.Enabled = false;
                    InternalTimer.Interval = 1;
                }
            }
        }

        private void OnElapse(object obj, ElapsedEventArgs e)
        {
            Signal = e.SignalTime;
            InternalTimer.Stop();
            _started = false;
            Elapsed = true;
            CompletionSource.SetResult(true);
        }

        public void Start()
        {
            if (!_started)
            {
                TimeStarted = DateTime.UtcNow;
                Signal = null;
                InternalTimer.Start();
                _started = true;
                Elapsed = false;
                //Console.WriteLine("Timer started.");
            }
        }

        public void Stop()
        {
            if (_started)
            {
                InternalTimer.Stop();
                Signal = DateTime.UtcNow;
                _started = false;
                Elapsed = false;
                //Console.WriteLine("Timer stopped.");
            }
        }

        public void Reset()
        {
            bool isActive = _started;
            Stop();
            
            if (isActive)
            {
                Start();
            }
        }
    }
}
