using Newtonsoft.Json;
using System;

namespace Arcadia
{
    // TODO: This entire class can be scrapped.
    // Items will be able to specify how boosters are determined
    public class BoostData
    {
        [JsonConstructor]
        public BoostData(string itemId, BoostTarget type, float rate, int? useLimit = null)
        {
            ParentId = itemId;
            Type = type;
            Rate = rate;
            UsesLeft = useLimit;
        }

        public BoostData(BoostTarget type, float rate, int useLimit)
        {
            Type = type;
            Rate = rate;
            UsesLeft = useLimit;
        }

        [JsonProperty("parent_id")]
        public string ParentId { get; }

        // If the type doesn't matter, this can be ignored
        [JsonProperty("type")]
        public BoostTarget Type { get; }

        // the rate of the booster
        [JsonProperty("rate")]
        public float Rate { get; }

        // how many times the booster has been activated
        [JsonProperty("uses_left")]
        public int? UsesLeft { get; internal set; }
    }
}
