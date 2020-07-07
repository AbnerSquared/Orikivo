using System.Collections.Generic;

namespace Orikivo.Desync
{

    // implement a sleep schedule

    // a RoutineNode is for a specific day.
    // the more nodes there are, the more unique scheduling there is.
    // RoutineSortOrder => Cycle (goes through each entry), Weekly (randomizes per week), Daily (randomizes until all unique ones are used), Random (randomized each time)
    /// <summary>
    /// Represents a daily system for how a <see cref="Character"/> moves around a <see cref="World"/>.
    /// </summary>
    public class Routine // routine timers need to be stored in the HuskBrain, to keep track of NPC routines.
    {
        public RoutineSortOrder Order { get; set; }

        public List<RoutineEntry> Entries { get; set; }
    }
}
