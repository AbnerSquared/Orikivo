using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    // this handles creating custom embeds, dedicated to resources.
    public class OriEmbedBuilder
    {
        // this defines the language that this embed is going to be in.
        public StringLocale Locale { get; set; }
    }


    // a string value with multiple values dedicated to lang.
    public class SourceString
    {
        public string Id { get; }
        public Dictionary<StringLocale, string> Locale { get; }
        public string ToString(StringLocale locale)
            => Locale[locale];
    }

    // the available languages
    public enum StringLocale
    {
        ENG = 1, // english
    }
}
