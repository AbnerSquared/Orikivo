using Newtonsoft.Json;
using System;

namespace Orikivo.Desync
{
    // TODO: Implement cancelling and updating at the % of completion.
    /// <summary>
    /// Represents the current travel process for a <see cref="Husk"/>.
    /// </summary>
    public class TravelData
    {
        [JsonConstructor]
        internal TravelData(LocationType type, string locationId, DateTime startedAt, DateTime arrival)
        {
            Type = type;
            Id = locationId;
            StartedAt = startedAt;
            Arrival = arrival;
        }

        // TODO: Remove usage of this LocationType.
        [JsonProperty("type")]
        public LocationType Type { get; set; }

        /// <summary>
        /// Gets the identifier that represents the specific location at which the <see cref="Husk"/> is travelling to.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets the <see cref="DateTime"/> at which the <see cref="Husk"/> started travelling to the specified location.
        /// </summary>
        [JsonProperty("started_at")]
        public DateTime StartedAt { get; set; }

        /// <summary>
        /// Gets the <see cref="DateTime"/> at which the <see cref="Husk"/> will arrive at the specified location.
        /// </summary>
        [JsonProperty("arrival")]
        public DateTime Arrival { get; set; }
        
        // NOTE: X and Y is used when in a FIELD or SECTOR, travelling to a specific coordinate
        [JsonProperty("x")]
        public float X { get; set; } = 0;

        [JsonProperty("y")]
        public float Y { get; set; } = 0;

        /// <summary>
        /// Gets a <see cref="bool"/> that defines if the <see cref="Husk"/> has finished travelling.
        /// </summary>
        [JsonIgnore]
        public bool Complete => (DateTime.UtcNow - Arrival).TotalSeconds > 0;
    }
}
