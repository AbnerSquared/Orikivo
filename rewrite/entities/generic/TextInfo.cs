using System.Collections.Generic;

namespace Orikivo
{
    /* Text strings that Orikivo uses to speak anything.*/
    public struct TextInfo
    {
        public string Key { get; }
        public Dictionary<TextLang, string[]> Values { get; }
    }
}
