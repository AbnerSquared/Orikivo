using Orikivo.Drawing;
using System.Collections.Generic;

namespace Orikivo.Unstable
{
    public class ConstructLayer
    {
        public int Level { get; set; }

        public string Name { get; set; }
        
        public Sprite Image { get; set; }

        public List<Npc> Npcs { get; set; }

        public ConstructTag Tag { get; set; } // maybe ConstructType instead
    }
}
