using System.Collections.Generic;
using System.Linq;

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

        public bool Exists(string id)
            => Nodes.Any(x => x.Id == id);

        public string GetString(string id, params object[] args)
            => GetNode(id)?.ToString(args) ?? "INVALID_LOCALE";
    }
}
