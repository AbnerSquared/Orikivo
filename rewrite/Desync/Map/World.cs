using Orikivo.Drawing;
using System.Collections.Generic;

namespace Orikivo.Unstable
{
    public class World
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Sprite Map { get; set; }

        public List<Field> Fields { get; set; }

        public List<Sector> Sectors { get; set; }

        public System.Drawing.SizeF Boundary { get; set; }

        public List<RegionF> Barriers { get; set; }

        public float DistanceRatio { get; set; }
    }
}
