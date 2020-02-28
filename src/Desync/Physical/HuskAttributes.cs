using Newtonsoft.Json;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a <see cref="Husk"/>'s current physical structure.
    /// </summary>
    public class HuskAttributes
    {
        public HuskAttributes() { }

        [JsonConstructor]
        internal HuskAttributes(int maxHp, int maxRange, int maxSpeed)
        {
            MaxHealth = maxHp;
            MaxSight = maxRange;
            MaxSpeed = maxSpeed;
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
    }
}
