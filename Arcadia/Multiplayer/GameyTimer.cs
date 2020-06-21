using System;
using System.Timers;


namespace Arcadia
{
    public class GameyTimer
    {
        public GameyTimer(TimeSpan duration, string actionId)
        {
            InternalTimer = new Timer
            {
                Enabled = false,
                AutoReset = false,
                Interval = duration.TotalMilliseconds,

            };

            Duration = duration;
            ActionId = actionId;

            _active = false;

            StartedAt = null;
            DurationElapsed = 0;
        }

        private bool _active;

        private Timer InternalTimer { get; }

        // How long does this timer go for?
        public TimeSpan Duration { get; set; }

        // how many milliseconds has this passed for.
        private double DurationElapsed { get; set; }

        // when did this timer invoke the method Start()??
        private DateTime? StartedAt { get; set; }

        // What is the action that is invoked when this timer goes off.
        public string ActionId { get; set; }

        private void OnElapse(object obj, ElapsedEventArgs e)
        {
            //Signal = e.SignalTime;
            InternalTimer.Stop();
            _active = false;
            
        }

        // start running the timer
        public void Start()
        {
            if (!_active)
            {
                // set the new interval to the remainder time that is left and start the timer.
                InternalTimer.Interval = Duration.TotalMilliseconds - DurationElapsed;
                InternalTimer.Start();

                StartedAt = DateTime.UtcNow;
                _active = true;
            }
        }

        // stop running the timer (no reset)
        public void Stop()
        {
            if (_active)
            {
                var stoppedAt = DateTime.UtcNow;
                InternalTimer.Stop();
                DurationElapsed = (stoppedAt - StartedAt.Value).TotalMilliseconds;

                StartedAt = null;
                _active = false;
            }
        }

        // sets the timer to 0 again. If the timer is running, it will continue running.
        // If the timer is stopped, it will remain stopped.
        public void Reset()
        {
            var isActive = _active;
            // I think the duration resets.
            Stop();
            DurationElapsed = 0;
            // only start the timer instantly IF the timer is currently active.
            if (isActive)
                Start();
        }

        // effectively removes this timer from the event handlers.
        public void Close()
        {

        }
    }
}
