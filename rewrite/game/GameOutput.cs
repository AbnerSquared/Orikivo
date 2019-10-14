namespace Orikivo
{
    /// <summary>
    /// Defines what a GameWindow is linked to within a game.
    /// </summary>
    public enum GameOutput
    {
        /// <summary>
        /// Specifies that the GameWindow is meant for the lobby.
        /// </summary>
        Lobby = 1,
        /// <summary>
        /// Specifies that the GameWindow is meant for the current game session.
        /// </summary>
        Game = 2,

        /// <summary>
        /// Specifies that the GameWindow is meant for the spectator of a current game session.
        /// </summary>
        Spec = 3
    }
}
