using Newtonsoft.Json;
using System.Collections.Generic;

namespace Orikivo.Unstable
{
    public class HuskBrain
    {
        public HuskBrain() { }
        [JsonConstructor]
        internal HuskBrain(List<Relationship> relations, List<string> discoveredAreaIds)
        {
            Relations = relations ?? new List<Relationship>();
            DiscoveredAreaIds = discoveredAreaIds ?? new List<string>();
        }

        [JsonProperty("relations")]
        public List<Relationship> Relations { get; } = new List<Relationship>();

        [JsonProperty("area_ids")]
        public List<string> DiscoveredAreaIds { get; } = new List<string>();

        // you should keep track of market purchases, so they know when an item is out of stock or whatknot


        // This is for events and such, where once the user completes certain tasks, they will be marked here.
        [JsonProperty("progression")]
        public Dictionary<string, long> ProgressFlags { get; } = new Dictionary<string, long>();
    }
}
