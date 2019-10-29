using Newtonsoft.Json;
using System.Collections.Generic;

namespace Orikivo
{
    /// <summary>
    /// Defines the cost and criteria for each tier within an upgrade.
    /// </summary>
    public class UpgradeTierInfo
    {
        [JsonConstructor]
        internal UpgradeTierInfo(ulong cost, List<AccountCriterion> criteria)
        {
            Cost = cost;
            Criteria = criteria;
        }

        [JsonProperty("cost")]
        public ulong Cost { get; internal set; }

        [JsonProperty("criteria")]
        public List<AccountCriterion> Criteria { get; internal set; }
    }
}
