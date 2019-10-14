using System;

namespace Orikivo
{
    /// <summary>
    /// A collection of tags that can be used to accurately define a user.
    /// </summary>
    [Flags]
    public enum UserTag
    {
        Empty = 0,
        /// <summary>
        /// Defines the user as the current game host.
        /// </summary>
        Host = 1,

        /// <summary>
        /// Defines the user as currently watching the game.
        /// </summary>
        Watching = 2
    }
}
