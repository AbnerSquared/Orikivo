using Newtonsoft.Json;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a cached <see cref="Location"/>.
    /// </summary>
    public class Locator
    {
        public Locator() { }

        [JsonConstructor]
        internal Locator(string worldId, string id, float x, float y)
        {
            WorldId = worldId;
            Id = id;
            X = x;
            Y = y;
        }

        /// <summary>
        /// Defines a direct reference to a <see cref="World"/>.
        /// </summary>
        [JsonProperty("world_id")]
        public string WorldId { get; set; }

        /// <summary>
        /// Represents the generic reference to a <see cref="Location"/>.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Represents a <see cref="Husk"/>'s relative x-coordinate in a <see cref="Location"/>.
        /// </summary>
        [JsonProperty("x")]
        public float X { get; set; }

        /// <summary>
        /// Represents a <see cref="Husk"/>'s relative y-coordinate in a <see cref="Location"/>.
        /// </summary>
        [JsonProperty("y")]
        public float Y { get; set; }

        /// <summary>
        /// Returns the <see cref="LocationType"/> at which the <see cref="Husk"/> is currently at.
        /// </summary>
        public LocationType GetInnerType()
            => GetLocation()?.Type ?? throw new ResultNotFoundException("Could not find a location with the specified ID.");

        /// <summary>
        /// Returns the name of the <see cref="Location"/> at which the <see cref="Husk"/> is currently at.
        /// </summary>
        public string GetInnerName()
            => GetLocation()?.Name ?? throw new ResultNotFoundException("Could not find a location with the specified ID.");

        public Location GetLocation()
            => Engine.World.Find(Id);

        /// <summary>
        /// Returns a string that summarizes this location.
        /// </summary>
        /// <returns></returns>
        public string Summarize()
        {
            return Engine.GetLocationSummary(Id);
        }
    }
}
