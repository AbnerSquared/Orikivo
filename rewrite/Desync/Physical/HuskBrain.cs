using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Unstable
{
    /// <summary>
    /// Represents a <see cref="Husk"/>'s memory.
    /// </summary>
    public class HuskBrain
    {
        public HuskBrain()
        {
            Relations = new Dictionary<string, float>();
            DiscoveredAreaIds = new List<string>();
            Maps = new Dictionary<string, byte[]>();
            Catalogs = new Dictionary<string, CatalogData>();
            Flags = new List<string>();
        }

        [JsonConstructor]
        internal HuskBrain(Dictionary<string, float> relations, List<string> discoveredAreaIds, Dictionary<string, CatalogData> catalogs, List<string> flags)
        {
            Relations = relations ?? new Dictionary<string, float>();
            DiscoveredAreaIds = discoveredAreaIds ?? new List<string>();
            Catalogs = catalogs ?? new Dictionary<string, CatalogData>();
            Flags = flags ?? new List<string>();
        }

        [JsonProperty("relations")]
        public Dictionary<string, float> Relations { get; } = new Dictionary<string, float>();

        [JsonProperty("area_ids")]
        public List<string> DiscoveredAreaIds { get; } = new List<string>();

        // you should keep track of market purchases, so they know when an item is out of stock or whatknot

        // keep track of market purchases.
        [JsonProperty("catalogs")]
        public Dictionary<string, CatalogData> Catalogs { get; } = new Dictionary<string, CatalogData>();

        public CatalogData GetOrGenerateCatalog(Market market)
        {
            if (!Catalogs.ContainsKey(market.Id))
            {
                Catalogs.Add(market.Id, market.GenerateCatalog().Compress());
            }
            else if (!market.IsActive(Catalogs[market.Id].GeneratedAt))
            {
                Catalogs[market.Id] = market.GenerateCatalog().Compress();
            }
            
            return Catalogs[market.Id];
        }


        public void AddOrUpdateRelationship(Relationship relationship)
        {
            if (!Relations.TryAdd(relationship.NpcId, relationship.Value))
                Relations[relationship.NpcId] = relationship.Value;
        }

        /// <summary>
        /// Represents a cache of maps as compressed bytes.
        /// </summary>
        [JsonProperty("maps")]
        public Dictionary<string, byte[]> Maps { get; set; }

        // Flags store the id of an event that they completed.
        /// <summary>
        /// Represents the <see cref="Husk"/>'s completion of a <see cref="World"/>'s storyline.
        /// </summary>
        [JsonProperty("flags")]
        public List<string> Flags { get; } = new List<string>();

        public Relationship GetOrCreateRelationship(Npc npc)
        {
            if (!Relations.ContainsKey(npc.Id))
                Relations.Add(npc.Id, 0.0f);

            return new Relationship(npc.Id, Relations[npc.Id]);
        }

        public bool HasFlag(string id)
            => Flags.Contains(id);

        public void SetFlag(string id)
        {
            if (!Flags.Contains(id))
                Flags.Add(id);
        }
    }
}
