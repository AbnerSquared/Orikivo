using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Desync
{
    // TODO: Remove Npcs.
    /// <summary>
    /// Represents a <see cref="Construct"/> with multiple stories.
    /// </summary>
    public class Highrise : Construct // a building with multiple layers
    {
        public List<Floor> Layers { get; set; }

        public Floor GetLevel(int level)
            => Layers.First(x => x.Level == level);

        public override List<Npc> Npcs => Layers.Select(x => x.Npcs).Merge();
    }
}
