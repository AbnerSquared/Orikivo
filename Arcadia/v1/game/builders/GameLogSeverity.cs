namespace Arcadia.Old
{
    public enum GameLogSeverity
    {
        /// <summary>
        /// This tells the GameLogger to log everything that happens within a game.
        /// </summary>
        Verbose = 1,

        /// <summary>
        /// This tells the GameLogger to ignore all warnings, only logging events.
        /// </summary>
        Ignore = 2
    }
}
