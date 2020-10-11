using System;

namespace Orikivo
{
    public class MatchOptions
    {
        public static readonly MatchOptions Default = new MatchOptions
        {
            ResetTimeoutOnAttempt = false,
            MaxAttempts = null,
            Timeout = TimeSpan.FromSeconds(10)
        };

        public TimeSpan? Timeout { get; set; }

        public int? MaxAttempts { get; set; }

        public bool ResetTimeoutOnAttempt { get; set; }
    }
}
