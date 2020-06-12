using Orikivo;
using System;
using System.Collections.Generic;

namespace Arcadia.Old
{
    // metadata about the element added.
    public class ElementMetadata
    {
        internal ElementMetadata(IElement element, string parentId = null)
        {
            Type = element.Type;
            Id = element.Id;
            ParentId = parentId;
            if (IsParent)
            {
                IElementGroup<IElement> group = (element as IElementGroup<IElement>);
                if (group.ElementCount > 0)
                {
                    List<ElementMetadata> metadata = new List<ElementMetadata>();
                    foreach (IElement child in group.Elements)
                        metadata.Add(new ElementMetadata(child, Id));
                    _children = metadata;
                }
            }
        }

        public ElementType Type { get; }
        public string ParentId { get; }
        public string Id { get; }

        private List<ElementMetadata> _children;

        public List<ElementMetadata> Children
            => IsParent ? _children : throw new Exception("The element specified is not a parent.");

        public bool HasParent => Check.NotNull(ParentId);

        public bool IsParent => Type == ElementType.Group;

        public override string ToString() // this can be optimized
            => IsParent ?
            HasParent ? $"elements:{ParentId}.{Id}" : $"elements:{Id}"
            : HasParent ? $"element:{ParentId}.{Id}" : $"element:{Id}";
    }
}
