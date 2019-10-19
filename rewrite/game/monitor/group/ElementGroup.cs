using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    public class ElementGroup : IElementGroup<Element>
    {
        internal ElementGroup(string id, ElementGroupConfig config = null)
        {
            Id = id;
            // Index = config.Index;
            // Immutable = config.Immutable;
            ContentFormatter = config.ContentFormatter;
            InvalidChars = config.InvalidChars ?? new List<char> { '|', '`', '*', '_', '~' };
            ElementFormatter = config.ElementFormatter;
            ElementSeparator = config.ElementSeparator ?? " ";
            Update(config.GetProperties());
        }

        public string Id { get; } // elements:id

        public int Priority { get; internal set; }
        public bool IsHidden { get; internal set; } = false;
        public bool Immutable { get; } // if the element can be deleted.
        
        public ElementType Type => ElementType.Group;
        public List<Element> Elements { get; } = new List<Element>();
        // just gets the pure content.
        public string Content
        {
            get
            {
                int skipLength = ElementCount > PageElementLimit ?
                    Page.HasValue ? Page.Value * PageElementLimit.Value
                        : ElementCount - PageElementLimit.Value : 0;
                // fill on empty
                return string.Join(ElementSeparator, Elements.Skip(skipLength).Select(x => x.ToString(Debug)));
            }
        }

        // config
        public string ContentFormatter { get; }
        public List<char> InvalidChars { get; }
        public string ElementFormatter { get; }
        public string ElementSeparator { get; }

        // properties
        public bool CanFormat { get; set; } // should only be enabled when there is no frame set.
        public bool CanUseInvalidChars { get; set; }
        public int? ContentLimit { get; set; }
        public int? Capacity { get; set; }
        public int? PageElementLimit { get; set; }

        // public bool FillPageEmpties { get; set; }
        // if the page should fill all empty values to the page element limit.
        public int? Page { get; set; } // in order for a page number to be set
        // there has to be a page element limit.

        public bool Debug { get; set; } = false;
        public int ElementCount => Elements.Count;
        public ElementMetadata Metadata => new ElementMetadata(this);

        // used to help separate all of its values.
        public ElementMetadata Add(Element element)
        {
            if (element == null)
                throw new Exception("You can't add an empty element into a group.");
            element.CanFormat = CanFormat;
            foreach (char c in InvalidChars)
                if (!element.InvalidChars.Contains(c))
                    element.InvalidChars.Add(c);
            element.CanUseInvalidChars = CanUseInvalidChars;
            element.ContentFormatter = ElementFormatter;
            Elements.Add(element);
            return new ElementMetadata(element, Id);
        }
        public ElementMetadata Add(string content)
            => Add(new Element(content, $"new-element{Elements.Count}"));
        public void Remove(string id)
        {
            Elements.Remove(GetElement(id));
        }

        public void Remove(int index)
        {
            Elements.RemoveAt(index);
        }

        public void Remove(ElementMetadata metadata)
        {
            Elements.Remove(GetElement(metadata.Id));
        }

        public void Clear()
        {
            Elements.Clear();
        }

        public Element ElementAt(int index)
            => Elements[index]; // gets the element at the specified index.
        public Element GetElement(string id)
            => Elements.First(x => x.Id == id); // gets the element with the specified id.

        public ElementMetadata GetMetadataFor(int index)
            => new ElementMetadata(Elements[index], Id);
        public ElementMetadata GetMetadataFor(string id)
            => new ElementMetadata(Elements.First(x => x.Id == id), Id);

        public void Update(ElementGroupProperties properties)
        {
            if (properties == null)
                throw new Exception("The group properties specified is empty.");
            if (properties.CanFormat != null)
            {
                CanFormat = properties.CanFormat ?? CanFormat;
                foreach (Element element in Elements)
                    element.CanFormat = CanFormat;
            }
            if (properties.CanUseInvalidChars != null)
            {
                CanUseInvalidChars = properties.CanUseInvalidChars ?? CanUseInvalidChars;
                foreach (Element element in Elements)
                    element.CanUseInvalidChars = CanUseInvalidChars;
            }
            ContentLimit = properties.ContentLimit ?? ContentLimit;
            Capacity = properties.Capacity ?? Capacity;
            PageElementLimit = properties.PageElementLimit ?? PageElementLimit;
            Page = properties.Page ?? Page;
        }
        public override string ToString()
            =>
            (Debug ? "[s]" : "") +
            (Content.Length > ContentLimit ?
            CanFormat ?
                string.Format(ContentFormatter, Content)
                : Content
            : throw new Exception("The content length is larger than its limit."))
            + (Debug ? "[/s]" : "");
    }
}
