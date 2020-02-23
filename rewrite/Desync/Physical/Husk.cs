using Newtonsoft.Json;
using Orikivo.Drawing;
using System;

namespace Orikivo.Unstable
{
    /// <summary>
    /// Represents a <see cref="User"/>'s physical entity in a <see cref="World"/>.
    /// </summary>
    public class Husk
    {
        // the husk always starts out in the headmaster's sector (Sector 27)
        public Husk(Locator locator)
        {
            ClaimedAt = DateTime.UtcNow;
            // TODO: Handle default backpack creation
            Attributes = new HuskAttributes { MaxSight = 15, MaxHealth = 10, MaxSpeed = 10 };
            Backpack = new Backpack(4);
            State = 0; // the husk isn't doing anything.
            Location = locator;
        }

        [JsonConstructor]
        internal Husk(DateTime claimedAt, HuskAttributes attributes, Backpack backpack, HuskState flag)
        {
            ClaimedAt = claimedAt;
            Attributes = attributes;
            Backpack = backpack;
            State = flag;
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
        public Backpack Backpack { get; private set; }
        // TODO: Make WorldItem variants that specify a slot size, which can take more space than others.

        /// <summary>
        /// Represents how a <see cref="Husk"/> currently exists.
        /// </summary>
        [JsonProperty("state")]
        public HuskState State { get; private set; }
        
        // Where the husk is currently located. This stores a sector/field, area, construct and market id.
        
        /// <summary>
        /// Represents where a <see cref="Husk"/> is currently located.
        /// </summary>
        [JsonProperty("location")]
        public Locator Location { get; private set; }

        /// <summary>
        /// Represents where a <see cref="Husk"/> is heading, if any.
        /// </summary>
        [JsonProperty("movement")]
        public MovementInfo Movement { get; internal set; }

        /// <summary>
        /// Represents where a <see cref="Husk"/> is located coordinate-wise within a <see cref="World"/>.
        /// </summary>
        [JsonProperty("position")]
        public Vector2 Position { get; internal set; }
        
    }

    public class MovementInfo
    {
        [JsonConstructor]
        internal MovementInfo(LocationType type, string locationId, DateTime startedAt, DateTime arrival)
        {
            Type = type;
            LocationId = locationId;
            StartedAt = startedAt;
            Arrival = arrival;
        }

        [JsonProperty("type")]
        public LocationType Type { get; set; }

        //
        [JsonProperty("id")]
        public string LocationId { get; set; }

        // the time at which they will arrive at the location.
        // you DO NOT update the user's current position until they either cancel
        // or they arrive.
        [JsonProperty("started_at")]
        public DateTime StartedAt { get; set; }

        [JsonProperty("arrival")]
        public DateTime Arrival { get; set; }

        // only used if the location type is an area.
        [JsonProperty("x")]
        public float X { get; set; } = 0;

        [JsonProperty("y")]
        public float Y { get; set; } = 0;

        [JsonIgnore]
        public bool Complete => (DateTime.UtcNow - Arrival).TotalSeconds > 0;
    }

    public class HuskStatus
    {
        // public List<Injury> Injuries {get; set;}
        // public 
    }
}
