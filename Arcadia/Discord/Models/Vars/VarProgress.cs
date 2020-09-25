using Newtonsoft.Json;

namespace Arcadia
{
    public class VarProgress
    {
        public VarProgress(long required)
        {
            Current = Current;
            Required = required;
        }

        [JsonConstructor]
        internal VarProgress(long current, long required)
        {
            Current = current;
            Required = required;
        }

        [JsonProperty("current")]
        public long Current { get; set; }

        [JsonProperty("required")]
        public long Required { get; }
    }
}