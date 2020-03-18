using System.Collections.Generic;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a class that determines what can be found in a <see cref="Field"/>.
    /// </summary>
    public class LootTable : IGenerationTable
    {
        public List<LootEntry> Entries { get; set; }
        IEnumerable<ITableEntry> IGenerationTable.Entries => Entries;
    }
}
