using Orikivo.Drawing;
using System.Collections.Generic;
using System.Drawing;

namespace Orikivo.Unstable
{
    public class Sector
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Vector2 Position { get; set; }

        public SectorScale Scale { get; set; }

        public List<Area> Areas { get; set; } // All areas must be conjoined

        public Sprite Map { get; set; }
    }
}
