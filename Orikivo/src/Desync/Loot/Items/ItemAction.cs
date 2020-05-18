using System;

namespace Orikivo.Desync
{

    /// <summary>
    /// Represents an action for an <see cref="Item"/>.
    /// </summary>
    public class ItemAction
    {
        /// <summary>
        /// Gets or sets the total amount of times that the <see cref="Item"/> can be used.
        /// </summary>
        public int? Durability { get; set; } = null;

        /// <summary>
        /// Gets or sets the cooldown that is applied to the <see cref="Item"/> used.
        /// </summary>
        public TimeSpan? Cooldown { get; set; } = null;

        /// <summary>
        /// Represents the method that is performed on a <see cref="User"/> when the <see cref="Item"/> is used.
        /// </summary>
        public Action<User> OnUse { get; set; } = null;

        /// <summary>
        /// Determines if the <see cref="Item"/> is automatically deleted when unusable.
        /// </summary>
        public bool DeleteOnBreak { get; set; } = true;

        /// <summary>
        /// Gets or sets the total amount of time that the <see cref="Item"/> exists for when it is first used.
        /// </summary>
        public TimeSpan? Decay { get; set; } = null;
    }
}
