using Newtonsoft.Json;
using System;

namespace Arcadia
{
    // for Arcadia
    public class BoosterData
    {
        public BoosterData(string itemId, BoosterType type, float rate, TimeSpan? duration = null, int? useLimit = null)
        {
            ParentId = itemId;
            Type = type;
            Rate = rate;

            if (duration.HasValue)
                ExpiresOn = DateTime.UtcNow.Add(duration.Value);

            UsesLeft = useLimit;
        }

        public BoosterData(BoosterType type, float rate, TimeSpan duration)
        {
            Type = type;
            Rate = rate;
            ExpiresOn = DateTime.UtcNow.Add(duration);
        }

        public BoosterData(BoosterType type, float rate, int useLimit)
        {
            Type = type;
            Rate = rate;
            UsesLeft = useLimit;
        }

        public BoosterData(BoosterType type, float rate, TimeSpan duration, int useLimit)
        {
            Type = type;
            Rate = rate;
            ExpiresOn = DateTime.UtcNow.Add(duration);
            UsesLeft = useLimit;
        }

        [JsonConstructor]
        internal BoosterData(string parentId, BoosterType type, float rate, DateTime? expiresOn, int? usesLeft)
        {
            ParentId = parentId;
            Type = type;
            Rate = rate;
            ExpiresOn = expiresOn;
            UsesLeft = usesLeft;
        }

        [JsonProperty("parent_id")]
        public string ParentId { get; }

        [JsonProperty("type")]
        public BoosterType Type { get; }

        // the rate of the booster
        [JsonProperty("rate")]
        public float Rate { get; }
        // when the booster was used.

        [JsonProperty("expires_on")]
        public DateTime? ExpiresOn { get; }

        // how many times the booster has been activated
        [JsonProperty("uses_left")]
        public int? UsesLeft { get; internal set; }
    }
}
