using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo.Text
{
    public class LocaleBank
    {
        internal LocaleBank(Language language, List<LocaleNode> nodes)
        {
            Language = language;
            Nodes = nodes;
        }

        public Language Language { get; }

        public IReadOnlyList<LocaleNode> Nodes { get; }

        public LocaleNode GetNode(string id)
            => Nodes.FirstOrDefault(x => x.Id == id);
    }
}
