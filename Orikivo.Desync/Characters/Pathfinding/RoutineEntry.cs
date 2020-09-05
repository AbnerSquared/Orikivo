using System.Collections.Generic;

namespace Orikivo.Desync
{
    public class RoutineEntry
    {
        public string Id { get; set; }
        public List<RoutineNode> Nodes { get; set; }

        public RoutineNode GetNext(int index)
        {
            if (++index >= Nodes.Count - 1)
            {
                return null;
            }

            return Nodes[index];
        }

        public RoutineNode GetInitial()
            => Nodes[0];
    }
}
