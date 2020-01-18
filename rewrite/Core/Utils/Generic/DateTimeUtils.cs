using System;

namespace Orikivo
{
    public static class DateTimeUtils
    {
        public static bool IsExpired(DateTime? date)
            => date.HasValue ? (DateTime.UtcNow - date.Value).TotalSeconds > 0 : false;

        public static DateTime GetTimeIn(TimeSpan duration) // could make DateTime thing
            => DateTime.UtcNow.Add(duration);

        public static TimeSpan TimeSince(DateTime time)
            => DateTime.UtcNow - time;
    }
}
