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

        public int Count => StackCount ?? 1;
        public string Id { get; }
        public int? StackCount { get; internal set; }

        /// <summary>
        /// Unique information about the item, if one is specified.
        /// </summary>
        public UniqueItemData Unique { get; }
    }
}
