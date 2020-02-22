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

        /// <summary>
        /// Represents the function used to convert the <see cref="EventMarker"/> into a readable <see cref="string"/>.
        /// </summary>
        public Func<EventContext, string> Writer { get; set; }
    }
}
