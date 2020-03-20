using Newtonsoft.Json;
using System;

namespace Orikivo.Desync
{

    public class BoosterData
    {
        public BoosterData(BoosterType type, float rate, TimeSpan? decayLength = null, int? useLimit = null)
        {
            Type = type;
            Rate = rate;

            if (decayLength.HasValue)
                ExpiresOn = DateTime.UtcNow.Add(decayLength.Value);

            if (useLimit.HasValue)
                UsesRemaining = useLimit.Value;
        }

        [JsonConstructor]
        internal BoosterData(BoosterType type, float rate, DateTime? expiresOn, int? usesRemaining)
        {
            Type = type;
            Rate = rate;
            ExpiresOn = expiresOn;
            UsesRemaining = UsesRemaining;
        }

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
        public int? UsesRemaining { get; }
    }
}
