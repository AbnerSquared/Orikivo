using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    /// <summary>
    /// A logger for a game that stores all events that occur.
    /// </summary>
    public class GameLogger
    {
        /// <summary>
        /// A collection of all of the stored entries logged.
        /// </summary>
        public List<GameLogEntry> Entries { get; } = new List<GameLogEntry>();

        public void Log(string value)
        {
            GameLogEntry entry = GameLogEntry.FromString(value);
            Entries.Add(entry);
            Console.WriteLine(value);
        }

        public override string ToString()
            => string.Join('\n', Entries.Select(x => x.ToString()));
    }
}
