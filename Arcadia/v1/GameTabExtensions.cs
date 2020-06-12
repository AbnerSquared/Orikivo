namespace Arcadia.Old
{
    /// <summary>
    /// Extension methods used for the GameTab.
    /// </summary>
    public static class GameTabExtensions
    {
        public static ElementMetadata SetElement(this GameTab tab, string elementId, string content)
            => tab.SetElement(elementId, new Element(content));
        public static ElementMetadata SetAtGroup(this GameTab tab, string groupId, string elementId, string content)
            => tab.SetAtGroup(groupId, elementId, new Element(content));
        public static ElementMetadata AddToGroup(this GameTab tab, string groupId, string content)
            => tab.AddToGroup(groupId, new Element(content));
        public static ElementMetadata InsertAtGroup(this GameTab tab, string groupId, int index, string content)
            => tab.InsertAtGroup(groupId, index, new Element(content));
        public static ElementMetadata Insert(this GameTab tab, int index, string content)
            => tab.Insert(index, new Element(content));
        public static ElementMetadata AddElement(this GameTab tab, string content)
            => tab.AddElement(new Element(content));
    }
}
