using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;

namespace Orikivo
{
    /// <summary>
    /// A collection of varied elements.
    /// </summary>
    public class ElementCluster
    {
        public ElementCluster() { }
        public ElementCluster(List<Element> elements = null, List<ElementGroup> groups = null, int? capacity = null)
        {
            Elements = elements ?? new List<Element>();
            Groups = groups ?? new List<ElementGroup>();
            Capacity = capacity;
        }

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

        public ElementMetadata RemoveFromGroup(string groupId, int index)
            => GetGroup(groupId).RemoveAt(index);
        public ElementMetadata RemoveFromGroup(string groupId, string elementId)
            => GetGroup(groupId).Remove(elementId);
        public void ClearElement(string elementId)
            => GetElement(elementId).Clear();

        public void ClearGroup(string groupId)
            => GetGroup(groupId).Clear();

        public void ClearAtGroup(string groupId, string elementId)
            => GetGroup(groupId).GetElement(elementId).Clear();
        public ElementMetadata RemoveElement(string elementId)
        {
            Element element = GetElement(elementId);
            Elements.Remove(element);
            return element.Metadata;
        }


        public ElementMetadata RemoveElement(int index)
        {
            ElementMetadata metadata = GetElement(index).Metadata;
            Elements.RemoveAt(index);
            return metadata;
        }

        public ElementUpdateResult Update(TabUpdatePacket packet) // change what's returned
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

                ElementUpdateError? possibleError = null;
                switch (packet.Method)
                {
                    case ElementUpdateMethod.Set:
                        possibleError = TestFor(packet, ElementUpdateError.ElementUnspecified, ElementUpdateError.ElementNotFound,
                            ElementUpdateError.ContentUnspecified);

                        return possibleError.HasValue ? ElementUpdateResult.FromError(possibleError.Value)
                            : ElementUpdateResult.FromSuccess(packet.Method, SetElement(packet.ElementId, packet.Element));

                    case ElementUpdateMethod.Add:
                        possibleError = TestFor(packet, ElementUpdateError.TabCapacityReached, ElementUpdateError.ContentUnspecified);

                        return possibleError.HasValue ? ElementUpdateResult.FromError(possibleError.Value)
                            : ElementUpdateResult.FromSuccess(packet.Method, AddElement(packet.Element));

                    case ElementUpdateMethod.Insert:
                        possibleError = TestFor(packet, ElementUpdateError.IndexUnspecified, ElementUpdateError.IndexOutOfRange,
                            ElementUpdateError.TabCapacityReached, ElementUpdateError.ContentUnspecified);

                        return possibleError.HasValue ? ElementUpdateResult.FromError(possibleError.Value)
                            : ElementUpdateResult.FromSuccess(packet.Method, Insert(packet.Index.Value, packet.Element));

                    case ElementUpdateMethod.Remove:
                        possibleError = TestFor(packet, ElementUpdateError.ElementUnspecified, ElementUpdateError.ElementNotFound);

                        return possibleError.HasValue ? ElementUpdateResult.FromError(possibleError.Value)
                            : ElementUpdateResult.FromSuccess(packet.Method, RemoveElement(packet.ElementId));

                    case ElementUpdateMethod.RemoveAt:
                        possibleError = TestFor(packet, ElementUpdateError.IndexUnspecified, ElementUpdateError.IndexOutOfRange);

                        return possibleError.HasValue ? ElementUpdateResult.FromError(possibleError.Value)
                            : ElementUpdateResult.FromSuccess(packet.Method, RemoveElement(packet.Index.Value));

                    case ElementUpdateMethod.SetAtGroup:
                        possibleError = TestFor(packet, ElementUpdateError.GroupUnspecified, ElementUpdateError.GroupNotFound,
                            ElementUpdateError.ElementUnspecified, ElementUpdateError.ElementNotFound, ElementUpdateError.ContentUnspecified);

                        return possibleError.HasValue ? ElementUpdateResult.FromError(possibleError.Value)
                            : ElementUpdateResult.FromSuccess(packet.Method, GetGroup(packet.GroupId).Set(packet.ElementId, packet.Element));

                    case ElementUpdateMethod.AddToGroup:
                        possibleError = TestFor(packet, ElementUpdateError.GroupUnspecified, ElementUpdateError.GroupNotFound,
                            ElementUpdateError.GroupCapacityReached, ElementUpdateError.ContentUnspecified);

                        return possibleError.HasValue ? ElementUpdateResult.FromError(possibleError.Value)
                            : ElementUpdateResult.FromSuccess(packet.Method, GetGroup(packet.GroupId).Add(packet.Element));

                    case ElementUpdateMethod.InsertAtGroup:
                        possibleError = TestFor(packet, ElementUpdateError.GroupUnspecified, ElementUpdateError.GroupNotFound,
                            ElementUpdateError.IndexUnspecified, ElementUpdateError.IndexOutOfRange, ElementUpdateError.GroupCapacityReached,
                            ElementUpdateError.ContentUnspecified);

                        return possibleError.HasValue ? ElementUpdateResult.FromError(possibleError.Value)
                            : ElementUpdateResult.FromSuccess(packet.Method, GetGroup(packet.GroupId).Insert(packet.Index.Value, packet.Element));

                    case ElementUpdateMethod.RemoveFromGroup:
                        possibleError = TestFor(packet, ElementUpdateError.GroupUnspecified, ElementUpdateError.GroupNotFound,
                            ElementUpdateError.ElementUnspecified, ElementUpdateError.ElementNotFound, ElementUpdateError.ImmutableElement);

                        return possibleError.HasValue ? ElementUpdateResult.FromError(possibleError.Value)
                            : ElementUpdateResult.FromSuccess(packet.Method, GetGroup(packet.GroupId).Remove(packet.ElementId));

                    case ElementUpdateMethod.RemoveAtFromGroup:
                        possibleError = TestFor(packet, ElementUpdateError.GroupUnspecified, ElementUpdateError.GroupNotFound,
                            ElementUpdateError.IndexUnspecified, ElementUpdateError.IndexOutOfRange, ElementUpdateError.ImmutableElement);

                        return possibleError.HasValue ? ElementUpdateResult.FromError(possibleError.Value)
                            : ElementUpdateResult.FromSuccess(packet.Method, GetGroup(packet.GroupId).RemoveAt(packet.Index.Value));

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

        // test for many errors, returning the first error occurance.
        public ElementUpdateError? TestFor(ElementUpdatePacket packet, params ElementUpdateError[] errors)
        {
            if (!(errors?.Length > 0))
                return null;

            foreach (ElementUpdateError error in errors)
                if (!Test(packet, error))
                    return error;
            return null;
        }

        // returns false if the test fails.
        public bool Test(ElementUpdatePacket packet, ElementUpdateError error)
        {
            switch (error)
            {
                case ElementUpdateError.ImmutableElement:
                    return Check.NotNull(packet.ElementId) ? Check.NotNull(packet.GroupId) ?
                        ContainsGroup(packet.GroupId) ? GetGroup(packet.GroupId).GetElement(packet.ElementId).Immutable : false :
                        ContainsElement(packet.ElementId) ? GetElement(packet.ElementId).Immutable : false : false;

                case ElementUpdateError.TabCapacityReached:
                    return CanAddElement;

                case ElementUpdateError.GroupCapacityReached:
                    return Check.NotNull(packet.GroupId) ? ContainsGroup(packet.GroupId) ?
                        GetGroup(packet.GroupId).CanAddElement : false : false;

                case ElementUpdateError.ElementNotFound:
                    return Check.NotNull(packet.ElementId) ? ContainsElement(packet.ElementId) : false;

                case ElementUpdateError.ElementUnspecified:
                    return Check.NotNull(packet.ElementId);

                case ElementUpdateError.GroupNotFound:
                    return Check.NotNull(packet.GroupId) ? ContainsGroup(packet.GroupId) : false;

                case ElementUpdateError.GroupUnspecified:
                    return Check.NotNull(packet.GroupId);

                case ElementUpdateError.ContentUnspecified:
                    return packet.Element != null;

                case ElementUpdateError.PacketNullReference:
                    return packet != null;

                case ElementUpdateError.IndexUnspecified:
                    return packet.Index.HasValue;

                case ElementUpdateError.IndexOutOfRange:
                    return packet.Index.HasValue ? Check.NotNull(packet.GroupId) ? ContainsGroup(packet.GroupId) ?
                        packet.Index.Value.IsInRange(GetGroup(packet.GroupId).ElementCount) : false :
                        packet.Index.Value.IsInRange(Count) : false;

                case ElementUpdateError.UnknownMethod:
                    return (int)packet.Method <= 13 /* ElementUpdateError.Max */ && (int)packet.Method >= 1 /* ElementUpdateError.Min */;

                default:
                    throw new Exception("The update error specified cannot be tested for.");
            }
        }

        public int? Capacity { get; }
        public bool CanAddElement => Capacity.HasValue ? (Capacity.Value + 1 <= Capacity.Value) : true;

        public int Count => Elements.Count + Groups.Count;

        public bool IsEmpty => (Elements?.Count ?? 0) == 0 && (Groups?.Count ?? 0) == 0;

        public Element GetElement(string id)
            => ContainsElement(id) ? Elements.First(x => x.Id == id) : null;

        public Element GetElement(int index)
            => ContainsElement(index) ? Elements.ElementAt(index) : null;

        public bool ContainsElement(string id)
            => Elements.Any(x => x.Id == id);

        public bool ContainsElement(int index)
            => index.IsInRange(Elements.Count);

        public ElementGroup GetGroup(string id)
            => ContainsGroup(id) ? Groups.First(x => x.Id == id) : null;

        public bool ContainsGroup(string id)
            => Groups.Any(x => x.Id == id);

        public string Content
        {
            get
            {
                Console.WriteLine("Writing cluster content...");
                StringBuilder sb = new StringBuilder();
                List<IElement> elements = Elements.Cast<IElement>().Concat(Groups).OrderByDescending(x => x.Priority).ToList();
                foreach (IElement element in elements)
                {
                    Console.Write(element.ToString());
                    sb.AppendLine(element.ToString());
                }
                return sb.ToString();
            }
        }

        public override string ToString()
            => Content;
    }
}
