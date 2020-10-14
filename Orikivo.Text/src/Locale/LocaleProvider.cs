using System;
using System.Collections.Generic;

namespace Orikivo.Text
{
    public class LocaleProvider
    {
        public IEnumerable<LocaleEntry> Entries { get; set; }

        public LocaleEntry GetEntry(string id)
        {
            throw new NotImplementedException();
        }

        public string GetEntry(string id, Language language)
        {
            // The parser needs to be handled here to get rid of any template placeholders in a locale string

            throw new NotImplementedException();
        }
    }
}