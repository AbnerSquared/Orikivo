using System;

namespace Orikivo
{
    /// <summary>
    /// Represents a special day tracked in an Orikivo.EventLog.
    /// </summary>
    public class LoggedEvent
    {
        public string Note { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public OccurrenceRate Occurrence { get; set; }
    }
}
