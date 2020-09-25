using System.Collections.Generic;

namespace Arcadia.Games
{
    public class CatanMap
    {
        public List<CatanLand> Lands { get; set; }

        public List<CatanPort> Ports { get; set; }

        public List<CatanCity> Cities { get; set; }

        public List<CatanRoad> Roads { get; set; }
    }
}