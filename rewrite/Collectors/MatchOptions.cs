using System;

namespace Orikivo
{
    public class MatchOptions
    {
        public static MatchOptions Default => new MatchOptions { ResetTimeoutOnAttempt = false, Timeout = TimeSpan.FromSeconds(10) };
        public TimeSpan? Timeout { get; set; }
        public bool ResetTimeoutOnAttempt { get; set; }

        public MatchAction Action { get; set; }
    }
}
