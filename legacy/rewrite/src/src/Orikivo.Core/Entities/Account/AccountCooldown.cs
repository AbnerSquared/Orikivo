using System;

namespace Orikivo
{
    public class AccountCooldown
    {
        public DateTime Daily { get; set; } // a datetime that correlates with a daily.
        public DateTime Command { get; set; } // a date time that goes off of by the last command executed.
        public DateTime LastSeen { get; set; } // A datetime that goes off of by last seen.
    }
}