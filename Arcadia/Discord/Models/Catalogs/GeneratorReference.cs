using System.Collections.Generic;

namespace Arcadia
{
    /// <summary>
    /// Represents generation data to determine the next <see cref="CatalogEntry"/>.
    /// </summary>
    internal class GeneratorReference
    {
        internal GeneratorReference(long tier)
        {
            Tier = tier;
        }

        internal long Tier { get; }

        internal int SpecialCount { get; set; }

        internal Dictionary<string, int> Items { get; } = new Dictionary<string, int>();

        internal Dictionary<string, int> Discounts { get; } = new Dictionary<string, int>();

        internal Dictionary<string, int> GroupCounters { get; } = new Dictionary<string, int>();
    }
}