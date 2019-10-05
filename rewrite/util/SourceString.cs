using System.Collections.Generic;

namespace Orikivo
{
    // a string value with multiple values dedicated to lang.
    public class SourceString
    {
        public string Id { get; }
        public Dictionary<StringLocale, string> Locale { get; }
        public string ToString(StringLocale locale)
            => Locale[locale];
    }
}
