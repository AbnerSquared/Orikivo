using System;

namespace Arcadia
{
    // This specifies how the item is deleted

    public class ItemAction
    {
        internal ItemAction() { }

        public int? Durability { get; set; }

        public TimeSpan? Cooldown { get; set; }

        public TimeSpan? Expiry { get; set; }

        public ExpiryTrigger ExpiryTrigger { get; set; } = ExpiryTrigger.Own;

        public CooldownCategory CooldownCategory { get; set; } = CooldownCategory.Item;

        public Func<ArcadeUser, UsageResult> Action { get; set; }

        public Func<ArcadeUser, bool> Criteria { get; set; }

        // What happens when this item is broken?
        // This can be used to apply a debuff.
        public Action<ArcadeUser> OnBreak { get; set; }

        // Is this item deleted when broken?
        public bool DeleteOnBreak { get; set; }

        public ItemDeleteMode DeleteMode { get; set; }
    }
}
