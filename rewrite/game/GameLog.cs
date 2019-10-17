namespace Orikivo
{
    //.Log($"[{DateTime.UtcNow.ToShortTimeString()}] #{Id}.ReceiverConnected = {receiver.Id}");
    // .Log($"[{DateTime.UtcNow.ToShortTimeString()}] #{Id}.DisplayUpdated");

    /// <summary>
    /// A GameLogger entry defining what occurred within a Game.
    /// </summary>
    public class GameLog
    {
        private GameLog() { }

        /// <summary>
        /// Creates a custom log entry from a string written.
        /// </summary>
        internal static GameLog FromString(string content)
            => new GameLog { Content = content, Type = GameLogType.Custom };

        // Add static constructors for other types: FromAction FromEvent FromException

        /// <summary>
        /// The content that was written.
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// The type of entry that was specified.
        /// </summary>
        public GameLogType Type { get; private set; }
    }
}
