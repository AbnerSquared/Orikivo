using System;

namespace Orikivo
{
    public class CollectionOptions
    {
        public static CollectionOptions Default = new CollectionOptions { Capacity = null, IncludeFailedMatches = false, Timeout = TimeSpan.FromSeconds(30), ResetTimeoutOnMatch = false };
        public TimeSpan? Timeout { get; set; }
        public bool ResetTimeoutOnMatch { get; set; } = false;
        public int? Capacity { get; set; } = null;
        public bool IncludeFailedMatches { get; set; } = false;
    }
}
