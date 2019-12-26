using System;
using System.Collections.Generic;

namespace Orikivo
{
    public class Element : IElement
    {
        internal Element(string content = null, string name = null, ElementConfig config = null)
        {
            Name = name;
            // Index = config.Index;
            // Immutable = config.Immutable;
            config = config ?? ElementConfig.Empty; // ??=
            ContentFormatter = config.ContentFormatter;
            InvalidChars = config.InvalidChars ?? new List<char> { '|', '`', '*', '_', '~' };
            Update(content, config.GetProperties());
        }
        public string Name { get; }

        public string Id => $"element.{Name}";
        public int Priority { get; internal set; }
        public bool IsHidden { get; internal set; } = false;
        public bool Immutable { get; } // set on config, if the element can be deleted.
        public ElementMetadata Metadata => new ElementMetadata(this);
        public ElementType Type => ElementType.Value; // used to help return the context to its former.
        private string _content;
        public string Content
        {
            get => _content;
            private set => _content = CanUseInvalidChars ? value/*.Escape(InvalidChars)*/ : value.Remove(InvalidChars);
        }
        public string ContentFormatter { get; internal set; }
        // only it being placed within a group permits updating its invalid chars.
        public List<char> InvalidChars { get; internal set; }

        // properties
        public bool CanFormat { get; set; }
        public bool CanUseInvalidChars { get; set; } // characters that could violate discord chars.
        public int? ContentLimit { get; set; }
        public bool AllowNewLine { get; set; } = false;
        public void Update(string content, ElementProperties properties = null)
        {
            // prevent newlines
            Content = Checks.NotNull(content) ? AllowNewLine ? content : content.Replace('\n', ' ') : "null";
            if (properties != null)
                Update(properties);
        }
        public void Update(ElementProperties properties)
        {
            if (properties == null)
                throw new Exception("The properties specified is empty.");
        }

        public void Clear()
        {
            Content = null;
        }
        public override string ToString()
            => (ContentLimit.HasValue ? Content.Length < ContentLimit : true) ? CanFormat ? string.Format(ContentFormatter ?? "{0}", Content) : Content
            : throw new Exception("The content length is larger than its limit.");
    }
}
