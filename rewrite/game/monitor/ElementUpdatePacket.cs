namespace Orikivo
{
    // We need to create updates that happen in relation to a timer...?

    // a packet defining what to update the element with.
    // this can be defined within game tasks and such.
    // ensure that GameProperties create an element update mechanic.

    ///<summary>
    /// An update packet that contains information on what to update an element within a tab with.
    ///</summary>
    public class ElementUpdatePacket
    {
        public ElementUpdatePacket(ElementUpdateMethod method, string groupId = null, string elementId = null, int? index = null, Element element = null)
        {
            Method = method;
            Element = element;
            GroupId = groupId;
            ElementId = elementId;
            Index = index;
        }

        public ElementUpdatePacket(Element element, ElementUpdateMethod method = ElementUpdateMethod.Add,
            string elementId = null, string groupId = null, int? index = null)
            : this(method, groupId, elementId, index, element) {}

        ///<summary>
        /// The method used when updating the specified element.
        ///</summary>
        public ElementUpdateMethod Method { get; set; }

        ///<summary>
        /// The position of the element to interact with. Useful when IDs aren't required.
        ///</summary>
        public int? Index { get; set; }

        ///<summary>
        /// The element group to interact with.
        ///</summary>
        public string GroupId { get; set; }

        ///<summary>
        /// The element to be updated.
        ///</summary>
        public string ElementId { get; set; }

        ///<summary>
        /// The element to update with. Primarily used to add new elements and so forth.
        ///</summary>
        public Element Element { get; set; }
    }
}
