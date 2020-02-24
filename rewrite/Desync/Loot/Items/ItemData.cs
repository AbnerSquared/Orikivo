using Newtonsoft.Json;
using Orikivo.Unstable;

namespace Orikivo
{
    /// <summary>
    /// Represents data for an <see cref="Item"/>.
    /// </summary>
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
            // TODO: Check if the ID points to an item that is stackable.
            Id = id;
            StackCount = stackCount;
        }

        internal ItemData(string id, UniqueItemData unique)
        {
            // TODO: Check if the ID points to an item that is unique.
            Id = id;
            Unique = unique;
        }

        /// <summary>
        /// Gets a 32-bit integer that represents the literal count of an <see cref="Item"/>.
        /// </summary>
        [JsonIgnore]
        public int Count => StackCount ?? 1;

        [JsonProperty("id")]
        public string Id { get; }

        /// <summary>
        /// Represents the stack of an <see cref="Item"/>, if the <see cref="Item"/> is static.
        /// </summary>
        [JsonProperty("stack_count")]
        public int? StackCount { get; internal set; }

        /// <summary>
        /// Represents the unique properties of an <see cref="Item"/>, if the <see cref="Item"/> is unique.
        /// </summary>
        [JsonProperty("unique")]
        public UniqueItemData Unique { get; }
    }
}
