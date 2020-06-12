namespace Arcadia.Old
{
    /// <summary>
    /// Defines the current state of a game.
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// The game is currently idle in the lobby.
        /// </summary>
        Inactive = 1,

        /// <summary>
        /// The game is currently playing a session.
        /// </summary>
        Active = 2
    }
}
