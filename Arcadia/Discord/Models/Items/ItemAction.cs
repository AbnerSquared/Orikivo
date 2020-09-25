using System;

namespace Arcadia
{
    public class ItemUsage
    {
        internal ItemUsage() { }

        public string Example { get; set; }

        public int? Durability { get; set; }

        public TimeSpan? Cooldown { get; set; }

        public CooldownMode CooldownMode { get; set; } = CooldownMode.Item;

        public TimeSpan? Expiry { get; set; }

        public ExpiryTrigger ExpiryTrigger { get; set; } = ExpiryTrigger.Own;

        public Func<UsageContext, UsageResult> Action { get; set; }

        // What happens when this item is broken?
        public Action<ArcadeUser> OnBreak { get; set; }

        public DeleteMode DeleteMode { get; set; } = DeleteMode.Break;
    }
}
