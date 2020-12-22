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
        /// Defines the length of time that the <see cref="Item"/> is disabled when used.
        /// </summary>
        public TimeSpan? Cooldown { get; set; }

        /// <summary>
        /// Specifies the cooldown target for this <see cref="Item"/>.
        /// </summary>
        public CooldownTarget CooldownTarget { get; set; } = CooldownTarget.Item;

        /// <summary>
        /// Represents the length of time that this <see cref="Item"/> can be used.
        /// </summary>
        public TimeSpan? Timer { get; set; }

        /// <summary>
        /// Specifies the expiration starting triggers for this <see cref="Item"/>.
        /// </summary>
        public ExpireTriggers ExpireTriggers { get; set; } = ExpireTriggers.Own;

        /// <summary>
        /// Represents the method that is invoked when an <see cref="Item"/> is used.
        /// </summary>
        public Func<UsageContext, UsageResult> Action { get; set; }

        /// <summary>
        /// Represents the method that is invoked when an <see cref="Item"/> is broken.
        /// </summary>
        public Action<ArcadeUser> OnBreak { get; set; }

        /// <summary>
        /// Specifies the deletion triggers for this <see cref="Item"/>.
        /// </summary>
        public DeleteTriggers DeleteMode { get; set; } = DeleteTriggers.Break;
    }
}
