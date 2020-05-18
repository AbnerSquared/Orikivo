using Newtonsoft.Json;
using Orikivo.Drawing;
using System;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a <see cref="User"/>'s physical entity in a <see cref="World"/>.
    /// </summary>
    public class Husk
    {
        // the husk always starts out in the main recovery center (Sector 27)
        public Husk(Locator locator)
        {
            ClaimedAt = DateTime.UtcNow;
            // TODO: Handle default backpack creation
            Attributes = new HuskAttributes { MaxSight = 15, MaxReach = 2, MaxHealth = 10, MaxSpeed = 10, MaxExposure = 5 };
            Backpack = new Backpack(4);
            Location = locator;
            Status = new HuskStatus(Attributes);
        }

        [JsonConstructor]
        internal Husk(DateTime claimedAt,
            HuskAttributes attributes,
            Backpack backpack,
            HuskStatus status,
            Locator location,
            Destination destination)
        {
            ClaimedAt = claimedAt;
            Attributes = attributes;
            Backpack = backpack;
            Status = status;
            Location = location;
            Destination = destination;
        }

        /// <summary>
        /// The time (in <see cref="DateTime.UtcNow"/>) at which this <see cref="Husk"/> was synchronized.
        /// </summary>
        [JsonProperty("claimed_at")]
        public DateTime ClaimedAt { get; }

        /// <summary>
        /// A list of attributes that defines the <see cref="Husk"/>'s current skillset.
        /// </summary>
        [JsonProperty("attributes")]
        public HuskAttributes Attributes { get; } = new HuskAttributes();

        /// <summary>
        /// Represents the <see cref="Husk"/>'s collection of physical items.
        /// </summary>
        [JsonProperty("backpack")]
        public Backpack Backpack { get; internal set; }
        // TODO: Make WorldItem variants that specify a slot size, which can take more space than others.

        /// <summary>
        /// Represents the <see cref="Husk"/>'s current physical wellness.
        /// </summary>
        [JsonProperty("status")]
        public HuskStatus Status { get; private set; }
        
        /// <summary>
        /// Represents where a <see cref="Husk"/> is currently located.
        /// </summary>
        [JsonProperty("location")]
        public Locator Location { get; internal set; }

        /// <summary>
        /// Represents where a <see cref="Husk"/> is heading, if any.
        /// </summary>
        [JsonProperty("destination")]
        public Destination Destination { get; internal set; }

        /// <summary>
        /// Generates a new <see cref="Memorial"/> at the <see cref="Husk"/>'s current position.
        /// </summary>
        public Memorial SetMemorial()
        {
            return new Memorial
            {
                Backpack = Backpack,
                Location = Location
            };
        }

        public void UseItem(Item item)
        {
            if (Backpack.Contains(item))
            {

            }
        }

        [JsonIgnore]
        public EntityHitbox Hitbox
            => new EntityHitbox(Location.X, Location.Y, Status.Sight, Status.Reach);
    }
}
