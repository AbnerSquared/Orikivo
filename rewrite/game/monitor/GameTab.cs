using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;

namespace Orikivo
{
    // the actual display. tasks can bind their display to a tab upon start.
    public class GameTab
    {
        public string Id { get; } // screen:{parent_id}#{id}
        public List<Element> Elements { get; } = new List<Element>();
        public List<ElementGroup> Groups { get; } = new List<ElementGroup>();
        public ElementMetadata SetElement(string elementId, Element newElement) { throw new NotImplementedException(); }
        public ElementMetadata SetElement(string elementId, string content) { throw new NotImplementedException(); }
        public ElementMetadata SetAtGroup(string groupId, string elementId, Element newElement) { throw new NotImplementedException(); }
        public ElementMetadata SetAtGroup(string groupId, string elementId, string content) { throw new NotImplementedException(); }
        public ElementMetadata AddToGroup(string groupId, Element newElement) { throw new NotImplementedException(); }
        public ElementMetadata AddToGroup(string groupId, string content) { throw new NotImplementedException(); }
        public ElementMetadata InsertAtGroup(string groupId, int index, string content) { throw new NotImplementedException(); }
        public ElementMetadata InsertAtGroup(string groupId, int index, Element newElement) { throw new NotImplementedException(); }
        public ElementMetadata Insert(int index, string content) { throw new NotImplementedException(); }
        public ElementMetadata Insert(int index, Element element) { throw new NotImplementedException(); }
        public ElementMetadata AddElement(Element element) { throw new NotImplementedException(); }
        public void RemoveFromGroup(string groupId, int index) { throw new NotImplementedException(); }
        public void RemoveFromGroup(string groupId, string elementId) { throw new NotImplementedException(); }
        public void ClearElement(string elementId) { throw new NotImplementedException(); }
        public void ClearGroup(string groupId) { throw new NotImplementedException(); }
        public void RemoveElement(string elementId) { throw new NotImplementedException(); }
        public void RemoveElement(int index) { throw new NotImplementedException(); }

        public bool Update(TabUpdatePacket packet)
        {
            foreach (ElementUpdatePacket elementPacket in packet.Packets)
                if (!Update(elementPacket).IsSuccess)
                    return false;
            return true;
        }
        public ElementUpdateResult Update(ElementUpdatePacket packet)
        {
            // check all method types and if the method can be done without error.
            throw new NotImplementedException();
        }

        public Element GetElement(string id)
            => Elements.First(x => x.Id == id);
        public ElementGroup GetGroup(string id)
            => Groups.First(x => x.Id == id);

        public string Content
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                return sb.ToString();
            }
        }

        public override string ToString()
            => Content;
    }
}
