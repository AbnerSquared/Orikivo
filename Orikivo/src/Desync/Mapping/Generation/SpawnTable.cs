using System.Collections.Generic;

namespace Orikivo.Desync
{
    public class SpawnTable : IGenerationTable
    {
        public List<SpawnEntry> Entries { get; set; }
        IEnumerable<ITableEntry> IGenerationTable.Entries => Entries;
    }
}
