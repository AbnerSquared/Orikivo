﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a <see cref="Husk"/>'s memory.
    /// </summary>
    public class HuskBrain
    {
        /// <summary>
        /// Initializes a new <see cref="HuskBrain"/>.
        /// </summary>
        public HuskBrain()
        {
            Relations = new Dictionary<string, float>();
            DiscoveredRegionIds = new List<string>();
            Maps = new Dictionary<string, byte[]>();
            Catalogs = new Dictionary<string, CatalogData>();
            Flags = new List<string>();
        }

        [JsonConstructor]
        internal HuskBrain(Dictionary<string, float> relations, List<string> discoveredRegionIds, Dictionary<string, CatalogData> catalogs, List<string> flags)
        {
            Relations = relations ?? new Dictionary<string, float>();
            DiscoveredRegionIds = discoveredRegionIds ?? new List<string>();
            Catalogs = catalogs ?? new Dictionary<string, CatalogData>();
            Flags = flags ?? new List<string>();
        }

        /// <summary>
        /// Represents a collection for the raw value of a <see cref="Relationship"/> with an <see cref="Npc"/>.
        /// </summary>
        [JsonProperty("relations")]
        public Dictionary<string, float> Relations { get; } = new Dictionary<string, float>();

        [JsonProperty("discovered_region_ids")]
        public List<string> DiscoveredRegionIds { get; set; }

        // this stores all of the unique names given to a region by you
        [JsonProperty("region_names")]
        public Dictionary<string, string> RegionNames { get; set; }

        

        public bool HasDiscovered(string id)
            => DiscoveredRegionIds.Contains(id);

        public void MarkAsDiscovered(string id)
        {
            if (!HasDiscovered(id))
                DiscoveredRegionIds.Add(id);
        }


        [JsonProperty("catalogs")]
        public Dictionary<string, CatalogData> Catalogs { get; } = new Dictionary<string, CatalogData>();

        /// <summary>
        /// Represents a cache of maps as compressed bytes.
        /// </summary>
        [JsonProperty("maps")]
        public Dictionary<string, byte[]> Maps { get; set; }

        /// <summary>
        /// Represents the <see cref="Husk"/>'s completion of a <see cref="World"/>'s storyline.
        /// </summary>
        [JsonProperty("flags")]
        public List<string> Flags { get; } = new List<string>();

        public CatalogData GetOrGenerateCatalog(Market market)
        {
            if (!Catalogs.ContainsKey(market.Id))
            {
                Catalogs.Add(market.Id, market.Catalog.Generate().Compress());
            }
            else if (!market.IsActive(Catalogs[market.Id].GeneratedAt))
            {
                Catalogs[market.Id] = market.Catalog.Generate().Compress();
            }
            
            return Catalogs[market.Id];
        }

        public void AddOrUpdateRelationship(Relationship relationship)
        {
            if (!Relations.TryAdd(relationship.NpcId, relationship.Value))
                Relations[relationship.NpcId] = relationship.Value;
        }

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
