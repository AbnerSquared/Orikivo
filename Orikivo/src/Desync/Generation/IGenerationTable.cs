using System.Collections.Generic;

namespace Orikivo.Desync
{
    public interface IGenerationTable
    {
        IEnumerable<ITableEntry> Entries { get; }
    }
}
