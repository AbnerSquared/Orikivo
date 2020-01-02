using System.Collections.Generic;

namespace Orikivo.Unstable
{
    public class HuskBrain
    {
        public List<Relation> Relations { get; } = new List<Relation>();
        public List<string> DiscoveredAreaIds { get; } = new List<string>();
    }
}
