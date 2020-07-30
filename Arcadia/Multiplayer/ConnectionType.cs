using Discord;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents the type of connection that a <see cref="ServerConnection"/> references.
    /// </summary>
    public enum ConnectionType
    {
        /// <summary>
        /// Represents a <see cref="ServerConnection"/> with an unknown origin.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Represents a <see cref="ServerConnection"/> that originated from an <see cref="IGuild"/>.
        /// </summary>
        Guild = 1,

        /// <summary>
        /// Represents a <see cref="ServerConnection"/> that originated from an <see cref="IUser"/>.
        /// </summary>
        Direct = 2
    }
}