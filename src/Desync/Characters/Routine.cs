using System.Collections.Generic;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a daily system for how an <see cref="Npc"/> moves around the <see cref="World"/>.
    /// </summary>
    public class Routine // routine timers need to be stored in the HuskBrain, to keep track of NPC routines.
    {
        /// <summary>
        /// If set to true, this marks the <see cref="Routine"/> to be a daily habit, which enforces that the <see cref="RoutineMarker"/> final position results in a total length of at most 24 hours.
        /// </summary>
        public bool Daily { get; set; }
        public List<RoutineMarker> Markers { get; set; }
    }
}
