namespace Orikivo
{
    //.Log($"[{DateTime.UtcNow.ToShortTimeString()}] #{Id}.ReceiverConnected = {receiver.Id}");
    // .Log($"[{DateTime.UtcNow.ToShortTimeString()}] #{Id}.DisplayUpdated");

    /// <summary>
    /// A GameLogger entry defining what occurred within a Game.
    /// </summary>
    public class GameLogEntry
    {
        private GameLogEntry() { }

        /// <summary>
        /// Creates a custom log entry from a string written.
        /// </summary>
        internal static GameLogEntry FromString(string content)
            => new GameLogEntry { Content = content, Type = LogEntryType.Custom };

        /// <summary>
        /// The content that was written.
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// The type of entry that was specified.
        /// </summary>
        public LogEntryType Type { get; private set; }
    }
}
