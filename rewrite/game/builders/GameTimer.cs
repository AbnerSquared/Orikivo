using System;
using System.Threading.Tasks;

namespace Orikivo
{
    public class GameTimer
    {
        internal GameTimer(TimeSpan timeout, GameTaskQueue onTimeout)
        {
            Timeout = timeout;
            OnTimeout = onTimeout;
        }
        public TimeSpan Timeout { get; }
        private GameTaskQueue OnTimeout { get; }
        public DateTime? StartedAt { get; private set; }
        public TimeSpan TimeElapsed => StartedAt.HasValue ? DateTime.UtcNow - StartedAt.Value : TimeSpan.Zero;
        public TimeSpan TimeLeft => Timeout >= TimeElapsed ? TimeSpan.Zero : Timeout - TimeElapsed;

        public async Task<GameTaskQueue> Run()
        {
            StartedAt = DateTime.UtcNow;
            await Task.Delay(Timeout);
            return OnTimeout;
        }
    }
}
