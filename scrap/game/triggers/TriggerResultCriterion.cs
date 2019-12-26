using System.Collections.Generic;

namespace Orikivo
{
    public class TriggerResultCriterion
    {
        public string RequiredTriggerId { get; }
        public List<ArgMatchCriterion> ArgCriteria { get; } = new List<ArgMatchCriterion>();
    }
}
