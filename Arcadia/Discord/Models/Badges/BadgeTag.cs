using System;

namespace Arcadia
{
    /// <summary>
    /// Defines a collection of traits that a <see cref="Badge"/> can have.
    /// </summary>
    [Flags]
    public enum BadgeTag
    {
        /// <summary>
        /// The merit specified is marked as a milestone.
        /// </summary>
        Milestone = 1,

        /// <summary>
        /// The merit specified is secret, which hides the quote and possible rewards.
        /// </summary>
        Secret = 2,

        /// <summary>
        /// The merit specified is exclusive, which means it cannot be unlocked through normal means.
        /// </summary>
        Exclusive = 4
    }
}
