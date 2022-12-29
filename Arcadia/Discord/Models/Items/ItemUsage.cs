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

        // TODO: Remove this property
        /// <summary>
        /// Represents the length of time that this <see cref="Item"/> can be used.
        /// </summary>
        public TimeSpan? Timer { get; set; }

        // If unspecified, do not allow equipping
        /// <summary>
        /// Represents the equipment target that this <see cref="Item"/> is placed in (optional).
        /// </summary>
        public EquipTarget? EquipTarget { get; set; }

        /// <summary>
        /// Represents the size of this <see cref="Item"/> when equipped (optional).
        /// </summary>
        public EquipSize EquipSize { get; set; } = EquipSize.Small;

        // TODO: Remove this property
        /// <summary>
        /// Specifies the expiration starting triggers for this <see cref="Item"/>.
        /// </summary>
        public ExpireTriggers ExpireTriggers { get; set; } = ExpireTriggers.Own;

        /// <summary>
        /// Specifies the set of triggers that will automatically activate this <see cref="Item"/>.
        /// </summary>
        public UsageTrigger Triggers { get; set; }

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
        public DeleteTriggers DeleteTriggers { get; set; } = DeleteTriggers.Break;
    }
}
