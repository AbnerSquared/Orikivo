using System.Collections.Generic;

namespace Orikivo
{
    // TODO: Calculate % of all users that have unlocked this merit.
    public class MeritInfo
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public MeritGroup? Group { get; set; }
        public List<AccountCriterion> Criteria { get; set; }
        public MeritRewardInfo OnSuccess { get; set; }
    }
}
