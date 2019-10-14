namespace Orikivo
{
    // null values will not update the element property

    public class ElementProperties
    {
        public ElementProperties(bool? canFormat = null, bool? canUseInvalidChars = null,
            int? contentLimit = null)
        {
            CanFormat = canFormat ?? Default.CanFormat;
            CanUseInvalidChars = canUseInvalidChars ?? Default.CanUseInvalidChars;
            ContentLimit = contentLimit ?? Default.ContentLimit;
        }
        public static ElementProperties Default => new ElementProperties(true, false);
        public bool? CanFormat { get; set; }
        // a list of characters that is escaped.
        public bool? CanUseInvalidChars { get; set; }

        public int? ContentLimit { get; set; }
    }
}
