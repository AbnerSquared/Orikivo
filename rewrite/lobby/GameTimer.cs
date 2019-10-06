using System;
using System.Threading.Tasks;

namespace Orikivo
{
    public class GameTimer
    {
        internal GameTimer(TimeSpan timeout, GameRoute onTimeout)
        {
            Timeout = timeout;
            OnTimeout = onTimeout;
        }
        public TimeSpan Timeout { get; }
        private GameRoute OnTimeout { get; }

        public async Task<GameRoute> Run()
        {
            await Task.Delay(Timeout);
            return OnTimeout;
        }
    }
}
