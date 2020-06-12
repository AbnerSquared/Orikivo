using System;
using System.Threading.Tasks;
using System.Timers;

namespace Arcadia
{
    /// <summary>
    /// Represents an asynchronous timer.
    /// </summary>
    public class AsyncTimer
    {
        private bool _stopped = false;
        private bool _started = false;

        public AsyncTimer(double? milliseconds = null)
        {
            InternalTimer = new Timer
            {
                Enabled = false
            };

            if (milliseconds.HasValue)
                InternalTimer.Interval = milliseconds.Value;
                
            InternalTimer.AutoReset = false;
            Timeout = null;
            TimeStarted = Signal = null;
            ElapseReader = new TaskCompletionSource<bool>();
            InternalTimer.Elapsed += OnElapse;
        }

        private Timer InternalTimer { get; }

        private DateTime? TimeStarted { get; set; }

        private DateTime? Signal { get; set; }

        public TimeSpan ElapsedTime => TimeStarted.HasValue ?
            DateTime.UtcNow - TimeStarted.Value :
            Signal.HasValue ? Signal.Value - TimeStarted.Value : TimeSpan.Zero;

        // this is what is updated to determine if the timer was elapsed
        public TaskCompletionSource<bool> ElapseReader { get; private set; }

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
                    InternalTimer.Interval = 0;
                }
            }
        }

        private void OnElapse(object obj, ElapsedEventArgs e)
        {
            Signal = e.SignalTime;
            InternalTimer.Stop();
            _stopped = true;

            ElapseReader.SetResult(true);
        }

        public void Start()
        {
            if (!_started)
            {
                TimeStarted = DateTime.UtcNow;
                InternalTimer.Start();
            }
        }

        public void Stop()
        {
            if (!_stopped)
            {
                InternalTimer.Stop();
                _stopped = true;
            }
        }

        public void Reset()
        {
            ElapseReader = new TaskCompletionSource<bool>();
            TimeStarted = null;
            Signal = null;

            if (_started)
            {
                _started = false;
                Start();
            }
        }
    }
}
