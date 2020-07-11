using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Desync
{
    public class ItemHistory
    {
        private readonly int _capacity;

        public ItemHistory()
        {
            _capacity = 32;
        }

        public IEnumerable<ItemLog> Entries { get; set; }

        public void Append(ItemLog log)
        {
            if (Entries.Count() >= _capacity)
                Entries = Entries.Shift(1);

            Entries = Entries.Prepend(log);
        }
    }
}
