using System.Collections.Generic;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a generic <see cref="Item"/> container with a capacity.
    /// </summary>
    public class Storage
    {
        public int Capacity { get; }

        public List<string> ItemIds { get; }
    }
}
