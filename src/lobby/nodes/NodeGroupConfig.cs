namespace Orikivo
{
    public class NodeGroupConfig : NodeConfig
    {
        // if the node updates or removes nodes based on their internal index,
        // as opposed to their index on the collection.
        public bool UseValueIndex { get; set; }
        // what is used to separate each individual value in the group when formatted to a map.
        public string ValueSeparator { get; set; }
        public int ValueLimit { get; set; }
        // value map is used to format each individual value in the group
        public string ValueMap { get; set; }
    }
}
