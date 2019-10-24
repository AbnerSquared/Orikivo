using System.Collections.Generic;

namespace Orikivo
{
    // TODO: Configure this to derive from Resources.
    // This is a class that contains its ID from the Resources with all known languages of it.
    public class StringResource
    {
        public string Id { get; }
        public Dictionary<StringLocale, string> Locale { get; }
        public string ToString(StringLocale locale)
            => Locale[locale];
    }
}
