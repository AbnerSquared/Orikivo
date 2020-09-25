using Newtonsoft.Json;
using System;
using Orikivo;

namespace Arcadia
{
    /// <summary>
    /// Represents a data reference for an <see cref="Item"/>.
    /// </summary>
    public class ItemData
    {
        /// <summary>
        /// Initializes a non-unique <see cref="ItemData"/> stack.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Item"/> to store.</param>
        /// <param name="stackCount">The stack count of the <see cref="Item"/> to store.</param>
        internal ItemData(string id, int stackCount)
        {
            if (ItemHelper.IsUnique(id))
                throw new Exception("Incorrect item data initializer used.");

            Id = id;
            TempId = KeyBuilder.Generate(5);
            StackCount = stackCount;
        }

        /// <summary>
        /// Initializes a unique <see cref="ItemData"/> stack.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Item"/> to store.</param>
        /// <param name="data">The unique data of the <see cref="Item"/> to store.</param>
        public ItemData(string id, UniqueItemData data)
        {
            if (!ItemHelper.IsUnique(id))
                throw new Exception("Incorrect item data initializer used.");

            Id = id;
            TempId = KeyBuilder.Generate(5);
            Data = data;
        }

        [JsonConstructor]
        internal ItemData(string id, bool locked, int? stackCount, UniqueItemData data, ItemSealData seal)
        {
            Id = id;
            TempId = KeyBuilder.Generate(5);
            Locked = locked;
            StackCount = stackCount;
            Data = data;
            Seal = seal;
        }

        /// <summary>
        /// Specifies the stack count of this <see cref="ItemData"/>.
        /// </summary>
        [JsonIgnore]
        public int Count => StackCount ?? 1;

        /// <summary>
        /// Represents the ID of the <see cref="Item"/> that was referenced.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; }

        [JsonIgnore]
        public string TempId { get; }

        /// <summary>
        /// If true, this <see cref="ItemData"/> stack cannot be used.
        /// </summary>
        [JsonProperty("locked")]
        public bool Locked { get; internal set; }

        /// <summary>
        /// Specifies the non-unique stack count of this <see cref="ItemData"/>.
        /// </summary>
        [JsonProperty("stack_count")]
        public int? StackCount { get; internal set; }

        /// <summary>
        /// Specifies the unique data of this <see cref="ItemData"/>.
        /// </summary>
        [JsonProperty("data")]
        public UniqueItemData Data { get; }

        /// <summary>
        /// Specifies the seal of this <see cref="ItemData"/> stack.
        /// </summary>
        [JsonProperty("seal")]
        public ItemSealData Seal { get; internal set; }
    }
}
