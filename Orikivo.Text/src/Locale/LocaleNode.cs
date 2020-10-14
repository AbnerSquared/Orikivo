using System.Collections.Generic;

namespace Orikivo.Text
{
    public class LocaleNode
    {
        public string Id { get; set; }
        public IEnumerable<string> Values { get; set; }
    }
}