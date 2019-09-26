namespace Orikivo
{
    // this defines the position and type of all nodes in the display.
    public class NodeMetadata
    {
        public NodeMetadata(NodeType type, int index, int? groupId = null)
        {
            Type = type;
            Index = index;
            GroupId = groupId;
        }
        public int Index { get; } // the index of the object's own position.
        public int? GroupId { get; }

        public NodeType Type { get; }
    }
}
