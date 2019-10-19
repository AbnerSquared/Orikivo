using System;

namespace Orikivo
{
    /// <summary>
    /// A collection of tags that define a user's generic game state.
    /// </summary>
    [Flags]
    public enum UserTag
    {
        Empty = 0,
        /// <summary>
        /// Marks a user as the current game host.
        /// </summary>
        Host = 1,

        /// <summary>
        /// Marks a user as currently spectating a game.
        /// </summary>
        Watching = 2,

        /// <summary>
        /// Marks a user as currently playing a game.
        /// </summary>
        Playing = 4
    }
}
