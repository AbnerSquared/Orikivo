using System;

namespace Orikivo
{
    public static class DateTimeUtils
    {
        public static bool IsExpired(DateTime? date)
            => date.HasValue ? (DateTime.UtcNow - date.Value).TotalSeconds > 0 : false;

        public static DateTime GetTimeIn(double seconds)
            => DateTime.UtcNow.AddSeconds(seconds);
    }
}
