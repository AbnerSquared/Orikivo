using Discord;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents the type of connection that an <see cref="IConnection"/> references.
    /// </summary>
    public enum ConnectionType
    {
        /// <summary>
        /// Represents an unknown origin.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Represents an <see cref="IConnection"/> that originated from an <see cref="IGuild"/>.
        /// </summary>
        Guild = 1,

        /// <summary>
        /// Represents an <see cref="IConnection"/> that originated from an <see cref="IUser"/>.
        /// </summary>
        Direct = 2
    }
}
