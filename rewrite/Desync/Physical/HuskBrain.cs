using Newtonsoft.Json;
using System.Collections.Generic;

namespace Orikivo.Unstable
{
    public class HuskBrain
    {
        public HuskBrain() { }
        [JsonConstructor]
        internal HuskBrain(List<Relation> relations, List<string> discoveredAreaIds)
        {
            Relations = relations ?? new List<Relation>();
            DiscoveredAreaIds = discoveredAreaIds ?? new List<string>();
        }

        [JsonProperty("relations")]
        public List<Relation> Relations { get; } = new List<Relation>();

        [JsonProperty("area_ids")]
        public List<string> DiscoveredAreaIds { get; } = new List<string>();
    }
}
