using System;

namespace Orikivo.Unstable
{
    public class ItemAction // this applies to both world and digital items.
    {
        public int? UseLimit { get; set; } = null;

        public TimeSpan? Cooldown { get; set; } = null;

        public Action<User> OnUse { get; set; } = null;

        public bool BreakOnLastUse { get; set; } = true;

        // a decay timer that starts when the item is first used.
        public TimeSpan? Decay { get; set; } = null;
    }
}
