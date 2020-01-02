using System.Collections.Generic;

namespace Orikivo.Unstable
{
    public class Area
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Construct> Constructs { get; set; }
        public List<Npc> Npcs { get; set; }
    }
}
