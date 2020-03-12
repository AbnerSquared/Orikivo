using System.Collections.Generic;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a class that determines what appears in a <see cref="Field"/>.
    /// </summary>
    public class FieldLootGenerator
    {
        public List<CreatureTag> Creatures { get; set; }
        public List<string> RelicIds { get; set; }
        public List<string> MineralIds { get; set; }
    }

}
