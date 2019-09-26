using System.Collections.Generic;

namespace Orikivo
{
    public class GimiResult : ICasinoResult
    {
        public bool IsSuccess { get; }
        public ulong Reward { get; }
        public List<StatUpdateInfo> StatsToChange { get; set; }

        // design the result info here
    }
}
