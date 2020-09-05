using Newtonsoft.Json;
using Orikivo.Drawing;
using System;

namespace Orikivo.Desync
{
    // TODO: Implement cancelling and updating at the % of completion.
    /// <summary>
    /// Represents a current travel process for a <see cref="Husk"/>.
    /// </summary>
    public class Destination : Locator
    {
        public Destination(Locator locator, DateTime startedAt, DateTime arrival)
        {
            WorldId = locator.WorldId;
            Id = locator.Id;
            X = locator.X;
            Y = locator.Y;
            StartedAt = startedAt;
            Arrival = arrival;
        }

        [JsonConstructor]
        internal Destination(string worldId, string id, float x, float y, DateTime startedAt, DateTime arrival)
        {
            WorldId = worldId;
            Id = id;
            X = x;
            Y = y;
            StartedAt = startedAt;
            Arrival = arrival;
        }

        [JsonProperty("path")]
        // this is referenced to determine where their position would be.
        public Route Path { get; set; }

        /// <summary>
        /// Gets the <see cref="DateTime"/> at which the <see cref="Husk"/> started travelling to the specified location.
        /// </summary>
        [JsonProperty("started_at")]
        public DateTime StartedAt { get; set; }

        // specifies the time this destination was paused, if any.
        // updates arrival time on resume.
        [JsonProperty("paused_at")]
        public DateTime? PausedAt { get; set; }

        /// <summary>
        /// Gets the <see cref="DateTime"/> at which the <see cref="Husk"/> will arrive at the specified location.
        /// </summary>
        [JsonProperty("arrival")]
        public DateTime Arrival { get; set; }

        /// <summary>
        /// Gets a <see cref="bool"/> that defines if the <see cref="Husk"/> has finished travelling.
        /// </summary>
        [JsonIgnore]
        public bool Complete => (DateTime.UtcNow - Arrival).TotalSeconds > 0;

        public void Pause()
        {
            // if specified, the destination will not proceed.
            PausedAt = DateTime.UtcNow;
        }

        private float GetDistance()
        {
            // this is used to determine the total distance of this destination
            // this is used to figure out the remaining distance to calculate the new arrival time
            // if this destination was paused.
            throw new NotImplementedException();
        }

        public Vector2 GetPosition()
        {
            // this is used to get their current position in the world based on the current time.
            throw new NotImplementedException();
        }
    }
}
