using System.Collections.Generic;

namespace Arcadia
{
    public class ItemGroup
    {
        // This is the ID that is prefixed on items
        public string ShortId { get; set; }

        // This is the ID of the group
        public string Id { get; set; }

        public string Icon { get; set; }

        public string Name { get; set; }

        // This is applied to the name before the name of the item
        public string Prefix { get; set; }

        // This is the default summary of an item if one isn't specified.
        public string Summary { get; set; }

        public Dictionary<string, long> Attributes { get; set; }
    }
}
