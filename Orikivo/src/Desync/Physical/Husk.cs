﻿using Newtonsoft.Json;
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
            Attributes = new HuskAttributes { MaxSight = 15, MaxHealth = 10, MaxSpeed = 10, MaxExposure = 5 };
            Backpack = new Backpack(4);
            Location = locator;
            Status = new HuskStatus(Attributes);
        }

        [JsonConstructor]
        internal Husk(DateTime claimedAt, HuskAttributes attributes, Backpack backpack, HuskStatus status)
        {
            ClaimedAt = claimedAt;
            Attributes = attributes;
            Backpack = backpack;
            Status = status;
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
        /// Represents the <see cref="Husk"/>'s current physical wellness.
        /// </summary>
        [JsonProperty("status")]
        public HuskStatus Status { get; private set; }
        
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
        public TravelData Movement { get; internal set; }

        /// <summary>
        /// Represents where a <see cref="Husk"/> is located coordinate-wise within a <see cref="World"/>.
        /// </summary>
        [JsonProperty("position")]
        public Vector2 Position { get; internal set; }
        
    }
}