using Newtonsoft.Json;
using System;

namespace Orikivo.Unstable
{
    public class Husk
    {
        // the husk always starts out in the headmaster's sector (Sector 27)
        public Husk()
        {
            ClaimedAt = DateTime.UtcNow;
            Backpack = new Backpack();
            Flag = 0; // the husk isn't doing anything.
        }

        [JsonConstructor]
        internal Husk(DateTime claimedAt, HuskAttributes attributes, Backpack backpack, HuskFlag flag)
        {
            ClaimedAt = claimedAt;
            Attributes = attributes;
            Backpack = backpack;
            Flag = flag;
        }

        /// <summary>
        /// The UTC time at which this <see cref="Husk"/> was claimed.
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
        public Backpack Backpack { get; private set; } = new Backpack();

        /// <summary>
        /// Represents what the <see cref="Husk"/> is currently doing at specific locations.
        /// </summary>
        [JsonProperty("flag")]
        public HuskFlag Flag { get; private set; }        
    }

    public class HuskAttributes
    {
        public HuskAttributes() { }

        [JsonConstructor]
        internal HuskAttributes(int maxHp, int intel)
        {
            MaxHp = maxHp;
            Intel = intel;
        }

        [JsonProperty("max_hp")]
        public int MaxHp { get; set; }

        [JsonProperty("intel")]
        public int Intel { get; set; }
    }

    public class HuskGear
    {

    }
}
