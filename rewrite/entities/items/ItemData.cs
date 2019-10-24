using Newtonsoft.Json;

namespace Orikivo
{
    // TODO: Create a possible ID reference within the data.
    // That way, a dictionary isn't required.
    public class ItemData
    {
        [JsonConstructor]
        internal ItemData(int? stackCount, UniqueItemData unique)
        {
            StackCount = stackCount;
            Unique = unique;
        }

        internal ItemData(int stackCount)
        {
            StackCount = stackCount;
        }

        internal ItemData(UniqueItemData unique)
        {
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
