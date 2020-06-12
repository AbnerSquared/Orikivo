using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcadia.Old
{
    /// <summary>
    /// A logger for a game that stores all events that occur.
    /// </summary>
    public class GameLogger
    {
        /// <summary>
        /// A collection of all of the stored entries logged.
        /// </summary>
        public List<GameLog> Entries { get; } = new List<GameLog>();

        public void Log(string value)
        {
            GameLog entry = GameLog.FromString(value);
            Entries.Add(entry);
            Console.WriteLine(value);
        }

        public override string ToString()
            => string.Join('\n', Entries.Select(x => x.ToString()));
    }
}
