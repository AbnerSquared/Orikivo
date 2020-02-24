using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Unstable
{
    /// <summary>
    /// Represents a <see cref="Construct"/> with multiple stories.
    /// </summary>
    public class ConstructGroup : Construct // a building with multiple layers
    {
        public List<ConstructLayer> Layers { get; set; }
        public ConstructLayer GetLevel(int level)
        {
            return Layers.First(x => x.Level == level);
        }

        public override List<Npc> Npcs => Layers.Select(x => x.Npcs).Merge();
    }

}
