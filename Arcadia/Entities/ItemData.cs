using Newtonsoft.Json;

namespace Arcadia
{
    public class ItemData
    {
        internal ItemData() { }

        [JsonConstructor]
        internal ItemData(int? stackCount, UniqueItemData data)
        {
            StackCount = stackCount;
            Data = data;
        }

        [JsonIgnore]
        public int Count => StackCount ?? 1;

        [JsonProperty("stack_count")]
        public int? StackCount { get; internal set; }

        [JsonProperty("data")]
        public UniqueItemData Data { get; internal set; }
    }
}
