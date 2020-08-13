namespace Arcadia
{
    public class ItemGroup
    {
        // This is the ID of the group
        public string Id { get; set; }

        // This is applied to the name before the name of the item
        public string Prefix { get; set; }

        // This is the default summary of an item if one isn't specified.
        public string Summary { get; set; }
    }
}