namespace Orikivo
{
    /// <summary>
    /// Defines what a game window is linked to within a game.
    /// </summary>
    public enum GameWindowOutput
    {
        /// <summary>
        /// Specifies that the window is meant for the lobby.
        /// </summary>
        Lobby = 1,
        
        /// <summary>
        /// Specifies that the window is meant for the current game session.
        /// </summary>
        Game = 2,

        /// <summary>
        /// Specifies that the window is meant for the spectator of a current game session.
        /// </summary>
        Spectate = 3
    }
}
