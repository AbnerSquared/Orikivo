using System.Collections.Generic;

namespace Orikivo
{
    public class ElementConfig : ElementProperties, IElementConfig
    {
        public ElementConfig(bool? canFormat = null, bool? canUseInvalidChars = null,
            int? contentLimit = null, string contentFormatter = null, List<char> invalidChars =null)
            : base(canFormat, canUseInvalidChars, contentLimit)
        {
            ContentFormatter = contentFormatter;
            InvalidChars = invalidChars;
        }

        public ElementConfig(string contentFormatter = null,
            List<char> invalidChars = null, ElementProperties properties = null)
            : this(properties?.CanFormat, properties?.CanUseInvalidChars,
                  properties?.ContentLimit, contentFormatter, invalidChars) { }
        public string ContentFormatter { get; set; }
        public List<char> InvalidChars { get; set; }

        public static ElementConfig Empty => new ElementConfig((bool?)null);

        public ElementProperties GetProperties()
            => new ElementProperties(CanFormat, CanUseInvalidChars, ContentLimit);
    }
}
