using System;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents the handling method for a <see cref="ReactionInput"/>.
    /// </summary>
    [Flags]
    public enum ReactionHandling
    {
        /// <summary>
        /// Handles only reactions that are added.
        /// </summary>
        Add = 1,

        /// <summary>
        /// Handles only reactions that are removed.
        /// </summary>
        Remove = 2,

        /// <summary>
        /// Handles any kind of reaction.
        /// </summary>
        Any = Add | Remove
    }
}
