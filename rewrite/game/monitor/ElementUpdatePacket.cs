namespace Orikivo
{
    // a packet defining what to update the element with.
    // this can be defined within game tasks and such.
    // ensure that GameProperties create an element update mechanic.
    public class ElementUpdatePacket
    {
        public ElementUpdatePacket(ElementUpdateMethod method, string groupId = null, string elementId = null, int? index = null, TabElement element = null)
        {
            Method = method;
            Element = element;
            GroupId = groupId;
            ElementId = elementId;
            Index = index;
        }
        public ElementUpdatePacket(TabElement element, ElementUpdateMethod method = ElementUpdateMethod.Add,
            string elementId = null, string groupId = null, int? index = null)
            : this(method, groupId, elementId, index, element) {}
        public ElementUpdateMethod Method { get; set; }
        public int? Index { get; set; }
        public string GroupId { get; set; }
        public string ElementId { get; set; }
        public TabElement Element { get; set; }
    }
}
