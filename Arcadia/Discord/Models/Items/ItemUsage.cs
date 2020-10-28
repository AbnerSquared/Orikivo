using System;

namespace Arcadia
{
    /// <summary>
    /// Represents usage information for an <see cref="Item"/>.
    /// </summary>
    public class ItemUsage
    {
        internal ItemUsage() { }

        /// <summary>
        /// Represents an example input for the usage of this <see cref="Item"/> (optional).
        /// </summary>
        public string Example { get; set; }

        /// <summary>
        /// Represents the health pool for this <see cref="Item"/> before breaking.
        /// </summary>
        public int? Durability { get; set; }

        /// <summary>
        /// Represents the duration that the <see cref="Item"/> is unusable when used.
        /// </summary>
        public TimeSpan? Cooldown { get; set; }

        /// <summary>
        /// Represents the cooldown mode for this <see cref="Item"/>.
        /// </summary>
        public CooldownMode CooldownMode { get; set; } = CooldownMode.Item;

        /// <summary>
        /// Represents the duration that this <see cref="Item"/> is usable.
        /// </summary>
        public TimeSpan? Expiry { get; set; }

        /// <summary>
        /// Represents the expiration trigger for this <see cref="Item"/>.
        /// </summary>
        public ExpiryTrigger ExpiryTrigger { get; set; } = ExpiryTrigger.Own;

        /// <summary>
        /// Represents the method that is invoked when an <see cref="Item"/> is used.
        /// </summary>
        public Func<UsageContext, UsageResult> Action { get; set; }

        /// <summary>
        /// Represents the method that is invoked when an <see cref="Item"/> is broken.
        /// </summary>
        public Action<ArcadeUser> OnBreak { get; set; }

        /// <summary>
        /// Represents the mode of deletion for this <see cref="Item"/>.
        /// </summary>
        public DeleteMode DeleteMode { get; set; } = DeleteMode.Break;
    }
}
