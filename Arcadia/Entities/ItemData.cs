using Newtonsoft.Json;
using System;

namespace Arcadia
{
    public class ItemData
    {
        internal ItemData(string id, int stackCount)
        {
            if (ItemHelper.IsUnique(id))
                throw new Exception("Incorrect item data initializer used.");

            Id = id;
            StackCount = stackCount;
        }

        public ItemData(string id, UniqueItemData data)
        {
            if (!ItemHelper.IsUnique(id))
                throw new Exception("Incorrect item data initializer used.");

            Id = id;
            Data = data;
        }

        [JsonConstructor]
        internal ItemData(string id, int? stackCount, UniqueItemData data)
        {
            Id = id;
            StackCount = stackCount;
            Data = data;
        }

        [JsonIgnore]
        public int Count => StackCount ?? 1;

        [JsonProperty("id")]
        public string Id { get; }

        [JsonProperty("stack_count")]
        public int? StackCount { get; internal set; }

        [JsonProperty("data")]
        public UniqueItemData Data { get; internal set; }
    }
}
