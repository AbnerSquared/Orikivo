namespace Orikivo
{
    // null values will not update the element property

    public class ElementProperties
    {
        public ElementProperties(bool? canFormat = null, bool? canUseInvalidChars = null,
            int? contentLimit = null)
        {
            CanFormat = canFormat ?? true;
            CanUseInvalidChars = canUseInvalidChars ?? false;
            ContentLimit = contentLimit;
        }
        public static ElementProperties Default => new ElementProperties(true, false);
        public bool? CanFormat { get; set; }
        // a list of characters that is escaped.
        public bool? CanUseInvalidChars { get; set; }

        public int? ContentLimit { get; set; }
    }
}
