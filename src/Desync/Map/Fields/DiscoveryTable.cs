using System.Collections.Generic;

namespace Orikivo.Desync
{
    public class DiscoveryTable
    {
        public List<CreatureTag> Creatures { get; set; }
        public List<string> RelicIds { get; set; }
        public List<string> MineralIds { get; set; }
    }

}
