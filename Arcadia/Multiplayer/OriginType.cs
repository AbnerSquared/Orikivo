namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents the origin of an <see cref="IConnection"/>.
    /// </summary>
    public enum OriginType
    {
        /// <summary>
        /// Represents an unknown origin.
        /// </summary>
        Unknown = 0,

        Server = 1,

        // Specifies that the server connection originated from the GameSession
        Session = 2
    }
}
