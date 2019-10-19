using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;

namespace Orikivo
{
    // the actual display. tasks can bind their display to a tab upon start.
    public class GameTab
    {
        public GameTab(GameTabProperties properties)
        {
            if (properties == null)
                return;

            Name = properties.Name;
            Elements = properties.Elements;
            Groups = properties.Groups;
            Capacity = properties.Capacity;
        }

        public string WindowId { get; internal set; }
        public string Name { get; }
        public string Id => $"{(Checks.NotNull(WindowId) ? $"{WindowId}:": "")}tab.{Name}";
        public List<Element> Elements { get; } = new List<Element>();
        public List<ElementGroup> Groups { get; } = new List<ElementGroup>();

        public ElementMetadata SetElement(string elementId, Element newElement)
        {
            GetElement(elementId).Update(newElement.Content);
            return GetElement(elementId).Metadata;
        }

        public ElementMetadata SetAtGroup(string groupId, string elementId, Element newElement)
        {
            GetGroup(groupId).GetElement(elementId).Update(newElement.Content);
            return GetGroup(groupId).GetMetadataFor(elementId);
        }

        public ElementMetadata AddToGroup(string groupId, Element newElement)
            => GetGroup(groupId).Add(newElement);

        public ElementMetadata InsertAtGroup(string groupId, int index, Element newElement)
            => GetGroup(groupId).Insert(index, newElement);

        public ElementMetadata Insert(int index, Element element)
        {
            Elements.Insert(index, element);
            return element.Metadata;
        }
        
        public ElementMetadata AddElement(Element element)
        {
            Elements.Add(element);
            return element.Metadata;
        }

        public void RemoveFromGroup(string groupId, int index)
            => GetGroup(groupId).RemoveAt(index);
        public void RemoveFromGroup(string groupId, string elementId)
            => GetGroup(groupId).Remove(elementId);
        public void ClearElement(string elementId)
            => GetElement(elementId).Clear();
        public void ClearGroup(string groupId)
            => GetGroup(groupId).Clear();
        public void RemoveElement(string elementId)
            => Elements.Remove(GetElement(elementId));
        public void RemoveElement(int index)
            => Elements.RemoveAt(index);

        public ElementUpdateResult Update(TabUpdatePacket packet)
        {
            ElementUpdateResult result = null;
            foreach (ElementUpdatePacket elementPacket in packet.Packets)
            {
                result = Update(elementPacket);
                if (!result.IsSuccess)
                    return result;
            }
            return result;
        }

        public ElementUpdateResult Update(ElementUpdatePacket packet)
        {
            try
            {
                if (packet == null)
                    return ElementUpdateResult.FromError(ElementUpdateError.PacketNullReference);
                switch (packet.Method)
                {
                    case ElementUpdateMethod.Set:
                        if (!Checks.NotNull(packet.ElementId))
                            return ElementUpdateResult.FromError(ElementUpdateError.ElementUnspecified);
                        if (!ContainsElement(packet.ElementId))
                            return ElementUpdateResult.FromError(ElementUpdateError.ElementNotFound);
                        if (packet.Element == null)
                            return ElementUpdateResult.FromError(ElementUpdateError.ContentUnspecified);

                        return ElementUpdateResult.FromSuccess(packet.Method, SetElement(packet.ElementId, packet.Element));

                    case ElementUpdateMethod.Add:
                        if (!CanAddElement)
                            return ElementUpdateResult.FromError(ElementUpdateError.TabCapacityReached);
                        if (packet.Element == null)
                            return ElementUpdateResult.FromError(ElementUpdateError.ContentUnspecified);

                        return ElementUpdateResult.FromSuccess(packet.Method, AddElement(packet.Element));

                    case ElementUpdateMethod.Insert:
                        if (!packet.Index.HasValue)
                            return ElementUpdateResult.FromError(ElementUpdateError.IndexUnspecified);
                        if (packet.Index.Value > Count - 1 || packet.Index.Value < 0)
                            return ElementUpdateResult.FromError(ElementUpdateError.IndexOutOfRange);
                        if (!CanAddElement)
                            return ElementUpdateResult.FromError(ElementUpdateError.TabCapacityReached);
                        if (packet.Element == null)
                            return ElementUpdateResult.FromError(ElementUpdateError.ContentUnspecified);

                        return ElementUpdateResult.FromSuccess(packet.Method, Insert(packet.Index.Value, packet.Element));

                    case ElementUpdateMethod.Remove:
                        if (!Checks.NotNull(packet.ElementId))
                            return ElementUpdateResult.FromError(ElementUpdateError.ElementUnspecified);
                        if (!ContainsElement(packet.ElementId))
                            return ElementUpdateResult.FromError(ElementUpdateError.ElementNotFound);

                        ElementMetadata metadata = GetElement(packet.ElementId).Metadata;
                        RemoveElement(packet.ElementId);
                        return ElementUpdateResult.FromSuccess(packet.Method, metadata);

                    case ElementUpdateMethod.RemoveAt:
                        if (!packet.Index.HasValue)
                            return ElementUpdateResult.FromError(ElementUpdateError.IndexUnspecified);
                        if (packet.Index.Value > Count - 1 || packet.Index.Value < 0)
                            return ElementUpdateResult.FromError(ElementUpdateError.IndexOutOfRange);

                        ElementMetadata atMetadata = Elements[packet.Index.Value].Metadata;
                        RemoveElement(packet.Index.Value);
                        return ElementUpdateResult.FromSuccess(packet.Method, atMetadata);

                    case ElementUpdateMethod.SetAtGroup:
                        if (!Checks.NotNull(packet.GroupId))
                            return ElementUpdateResult.FromError(ElementUpdateError.GroupUnspecified);
                        if (!ContainsGroup(packet.GroupId))
                            return ElementUpdateResult.FromError(ElementUpdateError.GroupNotFound);
                        if (!Checks.NotNull(packet.ElementId))
                            return ElementUpdateResult.FromError(ElementUpdateError.ElementUnspecified);
                        if (!GetGroup(packet.GroupId).ContainsElement(packet.ElementId))
                            return ElementUpdateResult.FromError(ElementUpdateError.ElementNotFound);
                        if (packet.Element == null)
                            return ElementUpdateResult.FromError(ElementUpdateError.ContentUnspecified);

                        return ElementUpdateResult.FromSuccess(packet.Method, GetGroup(packet.GroupId).Set(packet.ElementId, packet.Element));

                    case ElementUpdateMethod.AddToGroup:
                        if (!Checks.NotNull(packet.GroupId))
                            return ElementUpdateResult.FromError(ElementUpdateError.GroupUnspecified);
                        if (!ContainsGroup(packet.GroupId))
                            return ElementUpdateResult.FromError(ElementUpdateError.GroupNotFound);
                        if (!GetGroup(packet.GroupId).CanAddElement)
                            return ElementUpdateResult.FromError(ElementUpdateError.GroupCapacityReached);
                        if (packet.Element == null)
                            return ElementUpdateResult.FromError(ElementUpdateError.ContentUnspecified);

                        return ElementUpdateResult.FromSuccess(packet.Method, GetGroup(packet.GroupId).Add(packet.Element));

                    case ElementUpdateMethod.InsertAtGroup:
                        if (!Checks.NotNull(packet.GroupId))
                            return ElementUpdateResult.FromError(ElementUpdateError.GroupUnspecified);
                        if (!ContainsGroup(packet.GroupId))
                            return ElementUpdateResult.FromError(ElementUpdateError.GroupNotFound);
                        if (!packet.Index.HasValue)
                            return ElementUpdateResult.FromError(ElementUpdateError.IndexUnspecified);
                        if (packet.Index.Value > GetGroup(packet.GroupId).ElementCount - 1 || packet.Index.Value < 0)
                            return ElementUpdateResult.FromError(ElementUpdateError.IndexOutOfRange);
                        if (!GetGroup(packet.GroupId).CanAddElement)
                            return ElementUpdateResult.FromError(ElementUpdateError.GroupCapacityReached);
                        if (packet.Element == null)
                            return ElementUpdateResult.FromError(ElementUpdateError.ContentUnspecified);

                        return ElementUpdateResult.FromSuccess(packet.Method, GetGroup(packet.GroupId).Insert(packet.Index.Value, packet.Element));

                    case ElementUpdateMethod.RemoveFromGroup:
                        if (!Checks.NotNull(packet.GroupId))
                            return ElementUpdateResult.FromError(ElementUpdateError.GroupUnspecified);
                        if (!ContainsGroup(packet.GroupId))
                            return ElementUpdateResult.FromError(ElementUpdateError.GroupNotFound);
                        if (!Checks.NotNull(packet.ElementId))
                            return ElementUpdateResult.FromError(ElementUpdateError.ElementUnspecified);
                        if (!GetGroup(packet.GroupId).ContainsElement(packet.ElementId))
                            return ElementUpdateResult.FromError(ElementUpdateError.ElementNotFound);

                        ElementMetadata removeGroupMetadata = GetGroup(packet.GroupId).GetMetadataFor(packet.ElementId);
                        RemoveFromGroup(packet.GroupId, packet.ElementId);
                        return ElementUpdateResult.FromSuccess(packet.Method, removeGroupMetadata);

                    case ElementUpdateMethod.RemoveAtFromGroup:
                        if (!Checks.NotNull(packet.GroupId))
                            return ElementUpdateResult.FromError(ElementUpdateError.GroupUnspecified);
                        if (!ContainsGroup(packet.GroupId))
                            return ElementUpdateResult.FromError(ElementUpdateError.GroupNotFound);
                        if (!packet.Index.HasValue)
                            return ElementUpdateResult.FromError(ElementUpdateError.IndexUnspecified);
                        if (packet.Index.Value > GetGroup(packet.GroupId).ElementCount - 1 || packet.Index.Value < 0)
                            return ElementUpdateResult.FromError(ElementUpdateError.IndexOutOfRange);

                        ElementMetadata removeAtGroupMetadata = GetGroup(packet.GroupId).GetMetadataFor(packet.Index.Value);
                        GetGroup(packet.GroupId).RemoveAt(packet.Index.Value);
                        return ElementUpdateResult.FromSuccess(packet.Method, removeAtGroupMetadata);

                    case ElementUpdateMethod.ClearGroup:
                        return ElementUpdateResult.FromSuccess(packet.Method, null);

                    default:
                        return ElementUpdateResult.FromError(ElementUpdateError.UnknownMethod, "The method specified does not exist.");
                }
            }
            catch (Exception ex)
            {
                return ElementUpdateResult.FromError(ElementUpdateError.Exception, ex.Message);
            }
        }

        public int? Capacity { get; }
        public bool CanAddElement => Capacity.HasValue ? (Capacity.Value + 1 <= Capacity.Value) : true;

        public int Count => Elements.Count + Groups.Count;

        public Element GetElement(string id)
            => ContainsElement(id) ? Elements.First(x => x.Id == id) : null;

        public bool ContainsElement(string id)
            => Elements.Any(x => x.Id == id);

        public ElementGroup GetGroup(string id)
            => ContainsGroup(id) ? Groups.First(x => x.Id == id) : null;

        public bool ContainsGroup(string id)
            => Groups.Any(x => x.Id == id);
        
        public string Content
        {
            get
            {
                Console.WriteLine("writing tab content...");
                StringBuilder sb = new StringBuilder();
                List<IElement> elements = Elements.Cast<IElement>().Concat(Groups).OrderByDescending(x => x.Priority).ToList();
                foreach (IElement element in elements)
                    sb.AppendLine(element.ToString());
                return sb.ToString();
            }
        }

        public override string ToString()
            => Content;
    }
}
