using Orikivo.Drawing;
using System.Collections.Generic;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a region of wilderness in a <see cref="World"/>.
    /// </summary>
    public class Field : Location
    {
        public override LocationType Type => LocationType.Field;

        // This defines what it takes up based on the world location
        public RegionF Region { get; set; }

        public List<Structure> Structures { get; set; }

        public List<Barrier> Barriers { get; set; }

        public FieldBiome Biome { get; set; }

        public Sprite Image { get; set; }

        public FieldLootGenerator Generator { get; set; }

        public List<FieldRegion> Regions { get; set; }
    }
}
