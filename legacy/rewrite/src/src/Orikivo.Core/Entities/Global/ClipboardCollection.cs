using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    /// <summary>
    /// Represents a collection of Orikivo.Clipboard.
    /// </summary>
    public class ClipboardCollection : IContextCollection<Clipboard2>
    {
        public List<Clipboard2> Values { get; set; } = new List<Clipboard2>();

        public List<Clipboard2> FromAuthor(ulong id)
        {
            if (ContainsAuthor(id))
            {
                return Values.Where(x => x.Author.Id == id).ToList();
            }
            return new List<Clipboard2>();
        }

        public bool ContainsAuthor(ulong id)
            => Values.Where(x => x.Author.Id.Exists()).Any(x => x.Author.Id == id);
    }
}
