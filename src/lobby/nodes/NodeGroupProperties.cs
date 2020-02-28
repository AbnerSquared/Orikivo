namespace Orikivo
{
    public class NodeGroupProperties : NodeProperties
    {
        // the limit to the amount of values per page
        public int? PageValueLimit { get; set; }

        // the page to display in the node group
        public int? Page { get; set; }
    }
}
