namespace Orikivo
{
    public class ElementGroupProperties : ElementProperties
    {
        public ElementGroupProperties(bool? canFormat = null, bool? canUseInvalidChars = null,
            int? contentLimit = null, int? capacity = null, int? pageElementLimit = null,
            int? page = null) : base(canFormat, canUseInvalidChars, contentLimit)
        {
            Capacity = capacity ?? Default.Capacity;
            PageElementLimit = pageElementLimit ?? Default.PageElementLimit;
            Page = page ?? Default.Page;
        }

        public ElementGroupProperties(int? capacity = null, int? pageElementLimit = null,
            int? page = null, ElementProperties properties = null)
            : this(properties?.CanFormat, properties?.CanUseInvalidChars, properties?.ContentLimit,
                  capacity, pageElementLimit, page) {}

        public static new ElementGroupProperties Default
            => new ElementGroupProperties(null, 8, null, ElementProperties.Default);
        // how many elements can be held.
        public int? Capacity { get; set; }
        // how many elements can the page show
        public int? PageElementLimit { get; set; }
        // the page it's on.
        public int? Page { get; set; }

        public ElementGroupProperties GetProperties()
            => new ElementGroupProperties(CanFormat, CanUseInvalidChars, ContentLimit,
                Capacity, PageElementLimit, Page);
    }
}
