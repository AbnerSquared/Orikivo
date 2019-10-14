using System;
using System.Collections.Generic;

namespace Orikivo
{
    public class TabElement : IElement
    {
        internal TabElement(string id, string content = null, ElementConfig config = null)
        {
            Id = id;
            // Index = config.Index;
            // Immutable = config.Immutable;
            ContentFormatter = config.ContentFormatter;
            InvalidChars = config.InvalidChars ?? new List<char> { '|', '`', '*', '_', '~' };
            Update(content, config.GetProperties());
        }
        public string Id { get; } // element:{id}
        public int Priority { get; internal set; }
        public bool IsHidden { get; internal set; } = false;
        public bool Immutable { get; } // set on config, if the element can be deleted.
        public ElementType Type => ElementType.Value; // used to help return the context to its former.
        private string _content;
        public string Content
        {
            get => _content;
            private set => _content = CanUseInvalidChars ? value.Escape(InvalidChars) : value.Remove(InvalidChars);
        }
        public string ContentFormatter { get; internal set; }
        // only it being placed within a group permits updating its invalid chars.
        public List<char> InvalidChars { get; internal set; }

        // properties
        public bool CanFormat { get; set; }
        public bool CanUseInvalidChars { get; set; } // characters that could violate discord chars.
        public int? ContentLimit { get; set; }
        public void Update(string content, ElementProperties properties = null)
        {
            // prevent newlines
            Content = Checks.NotNull(content) ? content.Replace('\n', ' ') : "null";
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
        public string ToString(bool debug = false)
            =>
            (debug ? "[s]" : "") +
            (Content.Length > ContentLimit ?
            CanFormat ?
                string.Format(ContentFormatter, Content)
                : Content
            : throw new Exception("The content length is larger than its limit."))
            + (debug? "[/s]" : "");
    }
}
