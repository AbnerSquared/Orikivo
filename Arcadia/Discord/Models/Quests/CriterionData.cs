using Newtonsoft.Json;

namespace Arcadia
{
    public class CriterionData
    {
        internal CriterionData()
        {
            Complete = false;
            Value = null;
        }

        internal CriterionData(bool complete, long? value)
        {
            Complete = complete;
            Value = value;
        }

        // If criterion data values are removed when completed, then this complete property can be removed

        [JsonProperty("complete")]
        public bool Complete { get; internal set; }

        [JsonProperty("value")]
        public long? Value { get; internal set; }
    }
}
