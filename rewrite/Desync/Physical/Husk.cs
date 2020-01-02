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

        /// <summary>
        /// The UTC time at which this <see cref="Husk"/> was claimed.
        /// </summary>
        public DateTime ClaimedAt { get; }

        /// <summary>
        /// A list of attributes that defines the <see cref="Husk"/>'s current skillset.
        /// </summary>
        public HuskAttributes Attributes { get; } = new HuskAttributes();

        /// <summary>
        /// Represents the <see cref="Husk"/>'s collection of physical items.
        /// </summary>
        public Backpack Backpack { get; private set; } = new Backpack();

        /// <summary>
        /// Represents what the <see cref="Husk"/> is currently doing at specific locations.
        /// </summary>
        public HuskFlag Flag { get; private set; }        
    }

    public class HuskAttributes
    {
        public int MaxHp { get; set; }
        public int Intel { get; set; }
    }

    public class HuskGear
    {

    }
}
