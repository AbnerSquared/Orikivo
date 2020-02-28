using Orikivo.Drawing;
using System.Collections.Generic;

namespace Orikivo.Desync
{

    public class Field // a dangerous location
    {
        public string Id { get; set; }

        public string Name { get; set; }

        // This defines what it takes up based on the world location
        public RegionF Region { get; set; }

        public FieldBiome Biome { get; set; }

        public Sprite Image { get; set; }

        public DiscoveryTable Discoverables { get; set; }

        public List<FieldEffect> Effects { get; set; }
    }
}
