using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    public static class DateTimeExtensions
    {

        public static bool IsExpired(this DateTime? time)
        {
            if (time.HasValue)
                return (DateTime.UtcNow - time.Value).TotalSeconds > 0;

            return false;
        }

        public static bool IsExpiredIn(this DateTime? time, TimeSpan duration)
        {
            if (time.HasValue)
                return (DateTime.UtcNow - time.Value.Add(duration)).TotalSeconds > 0;

            return false;
        }
    }
}
