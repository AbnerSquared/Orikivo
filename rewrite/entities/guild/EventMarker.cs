using System;
using System.Collections.Generic;

namespace Orikivo
{
    /// <summary>
    /// Represents a parseable marker when writing events.
    /// </summary>
    public class EventMarker
    {
        public string Name { get; set; }
        public List<string> Aliases { get; set; } = new List<string>();

        // this is what is used to parse the events.
        public Func<EventContext, string> Writer { get; set; }
    }
}
