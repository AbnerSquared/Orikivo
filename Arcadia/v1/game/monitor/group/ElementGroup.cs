using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arcadia.Old
{
    public class ElementGroup : IElementGroup<Element>
    {
        internal ElementGroup(string name, ElementGroupConfig config = null)
        {
            Name = name;
            // Index = config.Index;
            // Immutable = config.Immutable;
            config = config ?? ElementGroupConfig.Empty;
            ContentFormatter = config.ContentFormatter;
            InvalidChars = config.InvalidChars ?? new List<char> { '|', '`', '*', '_', '~' };
            ElementFormatter = config.ElementFormatter;
            ElementSeparator = config.ElementSeparator ?? " ";
            Update(config.GetProperties());
        }

        public string Name { get; } // elements:id
        public string Id => $"elements.{Name}";
        public int Priority { get; internal set; }
        public bool IsHidden { get; internal set; } = false;
        public bool Immutable { get; } = true; // if the element can be deleted.

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

                int emptyLength = FillEmpties && PageElementLimit.HasValue ? PageElementLimit.Value - (ElementCount - skipLength) : 0;

                List<string> empties = new List<string>();
                for (int i = 0; i < emptyLength; i++)
                    empties.Add(string.Format(ElementFormatter, " "));

                // fill on empty
                return string.Join(ElementSeparator, Elements.Skip(skipLength).Select(x => x.ToString()).Concat(empties));
            }
        }

        // config
        public string ContentFormatter { get; }
        public List<char> InvalidChars { get; }
        public string ElementFormatter { get; }
        public string ElementSeparator { get; }

        public bool FillEmpties { get; set; } = false;

        // properties
        public bool CanFormat { get; set; } // should only be enabled when there is no frame set.
        public bool CanElementsFormat { get; set; } = true;
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

        private Element SyncElement(Element element)
        {
            if (element == null)
                throw new Exception("You can't add an empty element into a group.");
            element.CanFormat = CanElementsFormat;
            foreach (char c in InvalidChars)
                if (!element.InvalidChars.Contains(c))
                    element.InvalidChars.Add(c);
            //element.CanUseInvalidChars = CanUseInvalidChars;
            element.ContentFormatter = ElementFormatter;
            return element;
        }
        // used to help separate all of its values.
        public ElementMetadata Add(Element element)
        {
            Elements.Add(SyncElement(element));
            return new ElementMetadata(element, Id);
        }

        public ElementMetadata Insert(int index, Element element)
        {
            Elements.Insert(index, SyncElement(element));
            return new ElementMetadata(element, Id);
        }

        public ElementMetadata Add(string content)
            => Add(new Element(content, $"new-element{Elements.Count}"));
        public ElementMetadata Insert(int index, string content)
            => Insert(index, new Element(content, $"new-element{Elements.Count}"));

        public ElementMetadata Set(string id, Element element)
        {
            GetElement(id).Update(element.Content);
            return GetMetadataFor(id);
        }

        public ElementMetadata Set(string id, string content)
            => Set(id, new Element(content));

        public ElementMetadata Remove(string id)
        {
            ElementMetadata metadata = GetMetadataFor(id);
            Elements.Remove(GetElement(id));
            return metadata;
        }

        public ElementMetadata RemoveAt(int index)
        {
            ElementMetadata metadata = GetMetadataFor(index);
            Elements.RemoveAt(index);
            return metadata;
        }
        public ElementMetadata Remove(ElementMetadata metadata)
            => Remove(metadata.Id);

        public void Clear()
            => Elements.Clear();

        public Element ElementAt(int index)
            => Elements[index]; // gets the element at the specified index.
        public Element GetElement(string id)
            => Elements.First(x => x.Id == id); // gets the element with the specified id.

        

        public bool ContainsElement(string id)
            => Elements.Any(x => x.Id == id);

        public bool CanAddElement => Capacity.HasValue ? (Capacity.Value + 1 <= Capacity.Value) : true;

        public bool ContainsElement(int index)
            => index <= ElementCount - 1;

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
            => (ContentLimit.HasValue ? Content.Length < ContentLimit : true) ? CanFormat ? string.Format(ContentFormatter ?? "{0}", Content) : Content
            : throw new Exception("The content length is larger than its limit.");
    }
}
