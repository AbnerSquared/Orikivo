﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace Orikivo
{
    /// <summary>
    /// Defines the properties of an upgrade.
    /// </summary>
    public class UpgradeInfo
    {
        [JsonConstructor]
        internal UpgradeInfo(string id, string name, string summary, List<UpgradeTierInfo> tiers)
        {
            Id = id;
            Name = name;
            Summary = summary;
            Tiers = tiers;
        }

        [JsonProperty("id")]
        public string Id { get; internal set; }
        
        [JsonProperty("name")]
        public string Name { get; internal set; }

        [JsonProperty("summary")]
        public string Summary { get; internal set; }

        /// <summary>
        /// The tiers of this upgrade. For every value added is a new tier.
        /// </summary>
        [JsonProperty("tiers")]
        public List<UpgradeTierInfo> Tiers { get; internal set; }
    }
}
