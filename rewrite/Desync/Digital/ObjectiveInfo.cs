using System.Collections.Generic;

namespace Orikivo.Unstable
{
    // used to set up objectives.
    public class ObjectiveInfo
    {
        public int Rank { get; set; }
        public List<ObjectiveCriterion> Criteria { get; set; }
    }
}
