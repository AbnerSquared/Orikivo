using System.Collections.Generic;
using System.Linq;
using System;

namespace Orikivo
{
    // the actual display. tasks can bind their display to a tab upon start.
    public class GameTab
    {
        public string Id { get; } // screen:{parent_id}#{id}
        public List<TabElement> Elements { get; } = new List<TabElement>();
        public List<TabElementGroup> Groups { get; } = new List<TabElementGroup>();
        public ElementMetadata SetElement(string elementId, TabElement newElement) { throw new NotImplementedException(); }
        public ElementMetadata SetElement(string elementId, string content) { throw new NotImplementedException(); }
        public ElementMetadata SetAtGroup(string groupId, string elementId, TabElement newElement) { throw new NotImplementedException(); }
        public ElementMetadata SetAtGroup(string groupId, string elementId, string content) { throw new NotImplementedException(); }
        public ElementMetadata AddToGroup(string groupId, TabElement newElement) { throw new NotImplementedException(); }
        public ElementMetadata AddToGroup(string groupId, string content) { throw new NotImplementedException(); }
        public ElementMetadata InsertAtGroup(string groupId, int index, string content) { throw new NotImplementedException(); }
        public ElementMetadata InsertAtGroup(string groupId, int index, TabElement newElement) { throw new NotImplementedException(); }
        public ElementMetadata Insert(int index, string content) { throw new NotImplementedException(); }
        public ElementMetadata Insert(int index, TabElement element) { throw new NotImplementedException(); }
        public ElementMetadata AddElement(TabElement element) { throw new NotImplementedException(); }
        public void RemoveFromGroup(string groupId, int index) { throw new NotImplementedException(); }
        public void RemoveFromGroup(string groupId, string elementId) { throw new NotImplementedException(); }
        public void ClearElement(string elementId) { throw new NotImplementedException(); }
        public void ClearGroup(string groupId) { throw new NotImplementedException(); }
        public void RemoveElement(string elementId) { throw new NotImplementedException(); }
        public void RemoveElement(int index) { throw new NotImplementedException(); }

        public ElementUpdateResult Update(ElementUpdatePacket packet)
        {
            // check all method types and if the method can be done without error.
            throw new NotImplementedException();
        }

        public TabElement GetElement(string id)
            => Elements.First(x => x.Id == id);
        public TabElementGroup GetGroup(string id)
            => Groups.First(x => x.Id == id);

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
