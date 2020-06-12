using System.Collections.Generic;

namespace Arcadia.Old
{
    // config: properties that are static once built.
    public class ElementGroupConfig : ElementGroupProperties, IElementGroupConfig
    {
        public static ElementGroupConfig Empty => new ElementGroupConfig((bool?)null);

        public ElementGroupConfig(bool? canFormat = null,
            bool? canUseInvalidChars = null, int? contentLimit = null,
            int? capacity = null, int? pageElementLimit = null, int? page = null,
            string contentFormatter = null, string elementFormatter = null,
            string elementSeparator = null, List<char> invalidChars = null)
            : base(canFormat, canUseInvalidChars, contentLimit, capacity,
                  pageElementLimit, page)
        {
            ContentFormatter = contentFormatter;
            InvalidChars = invalidChars;
            ElementFormatter = elementFormatter;
            ElementSeparator = elementSeparator;
        }

        public ElementGroupConfig(string contentFormatter = null,
            List<char> invalidChars = null, string elementFormatter = null,
            string elementSeparator = null, ElementGroupProperties properties = null)
            : this(properties?.CanFormat, properties?.CanUseInvalidChars,
                  properties?.ContentLimit, properties?.Capacity, properties?.PageElementLimit,
                  properties?.Page,contentFormatter, elementFormatter, elementSeparator, invalidChars) {}
        public string ContentFormatter { get; set; }
        public List<char> InvalidChars { get; set; }
        public string ElementFormatter { get; set; }
        public string ElementSeparator { get; set; }
    }
}
