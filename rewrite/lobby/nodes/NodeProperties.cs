namespace Orikivo
{
    // this is used to update a node with presets
    // change Config to Properties, as Config is used for initial class building.
    public class NodeProperties
    {
        // if the node is permitted to use maps
        public bool? AllowMapping { get; set; }
        // if the node is mapped to a specific receiver
        public ulong? ReceiverId { get; set; }
        public bool? AllowFormatting { get; set; } // if the node can use discord formatting properties
        public int? Index { get; set; }
        public string Title { get; set; }
    }
}
