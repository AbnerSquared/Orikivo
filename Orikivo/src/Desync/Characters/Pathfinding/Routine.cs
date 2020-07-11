using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a daily system for how a <see cref="Character"/> moves around a <see cref="World"/>.
    /// </summary>
    public class Routine // routine timers need to be stored in the HuskBrain, to keep track of NPC routines.
    {
        public RoutineSortOrder Order { get; set; }

        public List<RoutineEntry> Entries { get; set; }

        public RoutineEntry GetEntry(string id)
            => Entries.FirstOrDefault(x => x.Id == id);

        public RoutineEntry GetNextEntry(string id)
        {
            switch (Order)
            {
                case RoutineSortOrder.Cycle:
                    int i = Entries.IndexOf(GetEntry(id));
                    
                    if (i >= Entries.Count - 1)
                        i = -1; // -1, due to ++i

                    return Entries[++i];

                /*
                case RoutineSortOrder.Shuffle:
                    // this would require an additional reference, think about how:
                    break;
                */

                default:
                    return Randomizer.Choose(Entries);
            }
        }
    }
}
