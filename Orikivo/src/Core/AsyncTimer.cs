using System;
using System.Threading.Tasks;
using System.Timers;

namespace Orikivo
{
    /// <summary>
    /// Represents an asynchronous timer.
    /// </summary>
    public class AsyncTimer
    {
        private bool _started;

        public AsyncTimer(TimeSpan? duration)
        {
            InternalTimer = new Timer
            {
                Enabled = false,
                AutoReset = false
            };

            Timeout = duration;
            TimeStarted = Signal = null;
            CompletionSource = new TaskCompletionSource<bool>();
            InternalTimer.Elapsed += OnElapse;
            Elapsed = false;
        }

        public bool Elapsed { get; private set; }

        public TimeSpan ElapsedTime => TimeStarted.HasValue
            ? Signal.HasValue
                ? Signal.Value - TimeStarted.Value
                : DateTime.UtcNow - TimeStarted.Value
            : Signal.HasValue
                ? DateTime.UtcNow - Signal.Value
                : TimeSpan.Zero;

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

        private Timer InternalTimer { get; }

        private DateTime? TimeStarted { get; set; }

        private DateTime? Signal { get; set; }

        public void Start()
        {
            if (_started)
                return;

            TimeStarted = DateTime.UtcNow;
            Signal = null;
            InternalTimer.Start();
            CompletionSource = new TaskCompletionSource<bool>();
            _started = true;
            Elapsed = false;
        }

        public void Stop()
        {
            if (!_started)
                return;

            InternalTimer.Stop();
            Signal = DateTime.UtcNow;
            _started = false;
            Elapsed = false;
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

        private void OnElapse(object obj, ElapsedEventArgs e)
        {
            Signal = e.SignalTime;
            InternalTimer.Stop();
            _started = false;
            Elapsed = true;
            CompletionSource.SetResult(true);
        }
    }
}
