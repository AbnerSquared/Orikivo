using Newtonsoft.Json;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a <see cref="Husk"/>'s current physical structure.
    /// </summary>
    public class HuskAttributes
    {
        internal static readonly string Health = "hp";
        internal static readonly string Sight = "sight";
        internal static readonly string Exposure = "exposure";
        internal static readonly string Speed = "speed";
        internal static readonly string Reach = "reach";

        public HuskAttributes() { }

        [JsonConstructor]
        internal HuskAttributes(int maxHp, int maxSight, int maxExposure, int maxSpeed, int maxReach)
        {
            MaxHealth = maxHp;
            MaxSight = maxSight;
            MaxExposure = maxExposure;
            MaxSpeed = maxSpeed;
            MaxReach = maxReach;
        }

        /// <summary>
        /// Represents a <see cref="Husk"/>'s max health pool. This determines how many hits they can take before being desynchronized.
        /// </summary>
        [JsonProperty("max_hp")]
        public int MaxHealth { get; set; }

        /// <summary>
        /// Represents a <see cref="Husk"/>'s max view range. This determines how many locations are visible from their current position.
        /// </summary>
        [JsonProperty("max_sight")]
        public int MaxSight { get; set; }

        // IN MINUTES
        /// <summary>
        /// Represents a <see cref="Husk"/>'s max resistance to wild exposure. This determines how long they can remain outside of a <see cref="Sector"/> before they start taking damage.
        /// </summary>
        [JsonProperty("max_exposure")]
        public int MaxExposure { get; set; }

        /// <summary>
        /// Represents a <see cref="Husk"/>'s max possible travel speed. This determines how much time it takes to travel in open areas.
        /// </summary>
        [JsonProperty("max_speed")]
        public int MaxSpeed { get; set; }

        /// <summary>
        /// Represents a <see cref="Husk"/>'s max possible interactive distance. This determines how close they have to be to an object or location to interact with it.
        /// </summary>
        [JsonProperty("max_reach")]
        public int MaxReach { get; set; }
    }
}
