namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents the type of connection that a connection references.
    /// </summary>
    public enum ConnectionType
    {
        /// <summary>
        /// Represents an unknown origin.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Represents a connection that originated from a guild.
        /// </summary>
        Guild = 1,

        /// <summary>
        /// Represents a connection that originated from a user.
        /// </summary>
        Direct = 2
    }
}
