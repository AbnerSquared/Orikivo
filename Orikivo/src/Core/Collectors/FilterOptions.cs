using System;

namespace Orikivo
{
    public class FilterOptions
    {
        public static readonly FilterOptions Default = new FilterOptions
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
