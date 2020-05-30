using System.Collections.Generic;

namespace Orikivo.Desync
{
    // could be repurposed for arcadia
    // used to set up objectives.
    public class ObjectiveInfo
    {
        public int Rank { get; set; }
        public List<ObjectiveCriterion> Criteria { get; set; }
    }
}
