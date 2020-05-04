using Newtonsoft.Json;
using System;
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
            KnownRegionIds = new List<string>();
            Maps = new Dictionary<string, byte[]>();
            Catalogs = new Dictionary<string, CatalogData>();
            Flags = new List<string>();
            Objectives = new List<ObjectiveData>();
            ResyncAt = null;
            Memorials = new List<Memorial>();
        }

        [JsonConstructor]
        internal HuskBrain(Dictionary<string, float> relations,
            List<string> knownRegionIds,
            Dictionary<string, byte[]> maps,
            Dictionary<string, CatalogData> catalogs,
            List<string> flags,
            List<ObjectiveData> objectives,
            DateTime? resyncAt,
            List<Memorial> memorials)
        {
            Relations = relations ?? new Dictionary<string, float>();
            KnownRegionIds = knownRegionIds ?? new List<string>();
            Maps = maps ?? new Dictionary<string, byte[]>();
            Catalogs = catalogs ?? new Dictionary<string, CatalogData>();
            Flags = flags ?? new List<string>();
            Objectives = objectives ?? new List<ObjectiveData>();
            ResyncAt = resyncAt;
            Memorials = memorials ?? new List<Memorial>();
        }

        /// <summary>
        /// Stores a list of all relationships for all known characters.
        /// </summary>
        [JsonProperty("relations")]
        public Dictionary<string, float> Relations { get; } = new Dictionary<string, float>();

        /// <summary>
        /// Stores a list of all known regions.
        /// </summary>
        [JsonProperty("known_region_ids")]
        public List<string> KnownRegionIds { get; set; }

        /// <summary>
        /// Stores a list of all unique names given to regions.
        /// </summary>
        [JsonProperty("region_names")]
        public Dictionary<string, string> RegionNames { get; set; }

        /// <summary>
        /// Stores a list of catalogs for all visited markets.
        /// </summary>
        [JsonProperty("catalogs")]
        public Dictionary<string, CatalogData> Catalogs { get; } = new Dictionary<string, CatalogData>();

        /// <summary>
        /// Stores a cache of maps as compressed bytes.
        /// </summary>
        [JsonProperty("maps")]
        public Dictionary<string, byte[]> Maps { get; set; }

        /// <summary>
        /// Represents the <see cref="Husk"/>'s completion of a <see cref="World"/>'s storyline.
        /// </summary>
        [JsonProperty("flags")]
        public List<string> Flags { get; } = new List<string>();

        /// <summary>
        /// Stores a list of currently active objectives.
        /// </summary>
        [JsonProperty("objectives")]
        public List<ObjectiveData> Objectives { get; set; }

        /// <summary>
        /// When specified, this determines the time at which the Husk will be resynchronized.
        /// </summary>
        [JsonProperty("resync_at")]
        public DateTime? ResyncAt { get; set; }
        // a husk will always be resynchronized at the nearest known recovery
        // center.

        /// <summary>
        /// Represents a list of desynchronizations a husk has gone through. They can be relocated to retrieve lost items.
        /// </summary>
        public List<Memorial> Memorials { get; set; }

        /// <summary>
        /// Returns a bool that defines if the specified region ID was discovered.
        /// </summary>
        public bool HasDiscoveredRegion(string id)
            => KnownRegionIds.Contains(id);

        /// <summary>
        /// Marks the specified region ID as discovered.
        /// </summary>
        public void IdentifyRegion(string id)
        {
            if (!HasDiscoveredRegion(id))
                KnownRegionIds.Add(id);
        }

        

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

        public void AddOrUpdateAffinity(AffinityData affinity)
        {
            if (!Relations.TryAdd(affinity.NpcId, affinity.Value))
                Relations[affinity.NpcId] = affinity.Value;
        }

        public AffinityData GetOrAddAffinity(Npc npc)
        {
            if (!Relations.ContainsKey(npc.Id))
                Relations.Add(npc.Id, 0.0f);

            return new AffinityData(npc.Id, Relations[npc.Id]);
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
