using Newtonsoft.Json;

namespace Orikivo
{
    // TODO: Create a possible ID reference within the data.
    // That way, a dictionary isn't required.
    public class ItemData
    {
        [JsonConstructor]
        internal ItemData(string id, int? stackCount, UniqueItemData unique)
        {
            Id = id;
            StackCount = stackCount;
            Unique = unique;
        }

        internal ItemData(string id, int stackCount)
        {
            Id = id;
            StackCount = stackCount;
        }

        internal ItemData(string id, UniqueItemData unique)
        {
            Id = id;
            Unique = unique;
        }

        [JsonIgnore]
        public int Count => StackCount ?? 1;

        [JsonProperty("id")]
        public string Id { get; }

        [JsonProperty("stack_count")]
        public int? StackCount { get; internal set; }

        /// <summary>
        /// Unique information about the item, if one is specified.
        /// </summary>
        [JsonProperty("unique")]
        public UniqueItemData Unique { get; }
    }
}
