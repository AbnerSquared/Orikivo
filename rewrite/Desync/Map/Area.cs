using Orikivo.Drawing;
using System.Collections.Generic;
using System.Drawing;

namespace Orikivo.Unstable
{
    public class Area
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public RegionF Region { get; set; }

        public Sprite Image { get; set; }

        public List<Construct> Constructs { get; set; }
    }
}
