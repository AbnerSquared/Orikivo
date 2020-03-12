using System.Collections.Generic;

namespace Orikivo.Desync
{
    public enum RoutineSortOrder
    {
        Cycle = 1,
        Weekly = 2,
        Daily = 3,
        Random = 4
    }

    // implement a sleep schedule

    // a RoutineNode is for a specific day.
    // the more nodes there are, the more unique scheduling there is.
    // RoutineSortOrder => Cycle (goes through each node), Weekly (randomizes per week), Daily (randomizes until all unique ones are used), Random (randomized each time)
    /// <summary>
    /// Represents a daily system for how an <see cref="Npc"/> moves around a <see cref="World"/>.
    /// </summary>
    public class Routine // routine timers need to be stored in the HuskBrain, to keep track of NPC routines.
    {
        /// <summary>
        /// If set to true, this marks the <see cref="Routine"/> to be a daily habit, which enforces that the <see cref="RoutineNode"/> final position results in a total length of at most 24 hours.
        /// </summary>
        public bool Daily { get; set; }
        public List<RoutineNode> Nodes { get; set; }
    }
}
