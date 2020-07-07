using System.Collections.Generic;

namespace Orikivo.Desync
{
    public class RoutineEntry
    {
        public List<RoutineNode> Nodes { get; set; }

        // once a routine ends, a new one (or the same one) is selected.
    }
}
