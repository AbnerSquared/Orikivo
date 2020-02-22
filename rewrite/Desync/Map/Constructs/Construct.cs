using Orikivo.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Unstable
{


    public class Construct // a simple building or such in an area.
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public Sprite Image { get; set; }
        public ConstructTag Tag { get; set; }

        public virtual List<Npc> Npcs { get; set; } = new List<Npc>();

        /// <summary>
        /// Represents the availability of this <see cref="Construct"/>. If none is specified, the <see cref="Construct"/> will always be available. (unimplemented)
        /// </summary>
        public virtual Schedule Schedule { get => throw new System.NotImplementedException(); }
    }

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
