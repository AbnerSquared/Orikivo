using System.Collections.Generic;

namespace Orikivo.Static
{
    public static class UnicodeDictionary
    {
        public static List<Dictionary<char, string>> All = new List<Dictionary<char, string>>
        {
            UnicodeIndex.Bold,
            UnicodeIndex.Italics,
            UnicodeIndex.Superscripts,
            UnicodeIndex.Subscripts
        };

        public static List<List<char>> Defaults = new List<List<char>>
        {
            UnicodeIndex.Letters,
            UnicodeIndex.Numbers,
            UnicodeIndex.Symbols
        };
    }
}