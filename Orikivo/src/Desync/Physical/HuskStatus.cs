using Newtonsoft.Json;
using System.Collections.Generic;

namespace Orikivo.Desync
{

    /// <summary>
    /// Represents the physical wellness of a <see cref="Husk"/>.
    /// </summary>
    public class HuskStatus
    {
        public HuskStatus(HuskAttributes attributes)
        {
            Attributes = new Dictionary<string, int>
            {
                { HuskAttributes.Health, attributes.MaxHealth },
                { HuskAttributes.Sight, attributes.MaxSight },
                { HuskAttributes.Exposure, attributes.MaxExposure },
                { HuskAttributes.Speed, attributes.MaxSpeed },
                { HuskAttributes.Reach, attributes.MaxReach }
            };
        }

        [JsonConstructor]
        internal HuskStatus(Dictionary<string, int> attributes)
        {
            Attributes = attributes;
        }

        /// <summary>
        /// Represents the current state of attributes defined by <see cref="HuskAttributes"/>.
        /// </summary>
        [JsonProperty("attributes")]
        public Dictionary<string, int> Attributes { get; set; }

        // Represents a list of effects that might be applied to a Husk.
        [JsonProperty("effects")]
        public List<Effect> Effects { get; set; }

        // Not sure how i want upgrades to work
        // maybe upgrading the AI and the physical body could be 2 different things.
        [JsonProperty("upgrades")]
        public Dictionary<string, int> Upgrades { get; } = new Dictionary<string, int>();

        /// <summary>
        /// Represents a <see cref="Husk"/>'s current health.
        /// </summary>
        [JsonIgnore]
        public int Health
        {
            get => Attributes[HuskAttributes.Health];
            set => Attributes[HuskAttributes.Health] = value;
        }

        /// <summary>
        /// Determines if the <see cref="Husk"/> attached to this <see cref="HuskStatus"/> is dead.
        /// </summary>
        public bool Dead => Health <= 0;


        [JsonIgnore]
        public int Sight
        {
            get => Attributes[HuskAttributes.Sight];
            set => Attributes[HuskAttributes.Sight] = value;
        }

        [JsonIgnore]
        public int Exposure
        {
            get => Attributes[HuskAttributes.Exposure];
            set => Attributes[HuskAttributes.Exposure] = value;
        }

        [JsonIgnore]
        public int Speed
        {
            get => Attributes[HuskAttributes.Speed];
            set => Attributes[HuskAttributes.Speed] = value;
        }

        [JsonIgnore]
        public int Reach
        {
            get => Attributes[HuskAttributes.Reach];
            set => Attributes[HuskAttributes.Reach] = value;
        }

        public bool HasUpgradeAt(string id, int tier)
            => Upgrades.ContainsKey(id) ? Upgrades[id] == tier : false;

        // tick an upgrade by 1, if possible.
        public void Upgrade(string id)
        {
            if (!Upgrades.TryAdd(id, 1))
                Upgrades[id] += 1;
        }

        public int GetUpgradeTier(string id)
            => Upgrades.ContainsKey(id) ? Upgrades[id] : 0;

        // public List<Injury> Injuries {get; set;}
    }
}
