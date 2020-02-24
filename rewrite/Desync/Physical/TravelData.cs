using Newtonsoft.Json;
using System;

namespace Orikivo.Unstable
{
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

        [JsonProperty("type")]
        public LocationType Type { get; set; }

        //
        [JsonProperty("id")]
        public string Id { get; set; }

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
}
