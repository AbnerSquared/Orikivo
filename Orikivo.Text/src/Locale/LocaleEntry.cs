using System.Collections.Generic;
using System.Text;

namespace Orikivo.Text
{
    public class LocaleEntry
    {
        public Language Language { get; set; }
        public IEnumerable<LocaleNode> Nodes { get; set; }
    }
}
