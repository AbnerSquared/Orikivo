using Orikivo.Drawing;
using System.Collections.Generic;
using System.Linq;

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

        public List<Construct> Constructs { get; set; }

        public List<Area> Areas { get; set; }

        public List<Barrier> Barriers { get; set; }

        public BiomeType Biome { get; set; }

        public SpawnTable Creatures { get; set; }

        public LootTable Loot { get; set; }

        public List<Biome> Biomes { get; set; }

        /// <summary>
        /// Represents the base <see cref="Desync.Atmosphere"/> for this <see cref="Field"/>.
        /// </summary>
        public Atmosphere Atmosphere { get; set; }

        public Area GetArea(string id)
            => Areas?.FirstOrDefault(x => x.Id == id);

        public Construct GetConstruct(string id)
            => Constructs?.FirstOrDefault(x => x.Id == id);

        public Structure GetStructure(string id)
            => Structures?.FirstOrDefault(x => x.Id == id);

        public override List<Location> GetChildren(bool includeInnerChildren = true)
        {
            var children = new List<Location>();

            if (Areas?.Count > 0)
            {
                children.AddRange(Areas);

                if (includeInnerChildren)
                    foreach (Area area in Areas)
                        children.AddRange(area.GetChildren());
            }

            if (Constructs?.Count > 0)
                children.AddRange(Constructs);

            return children;
        }

        public override List<Region> GetRegions()
        {
            var regions = new List<Region>();
            regions.AddRange(Biomes);
            regions.AddRange(Constructs);
            regions.AddRange(Structures);
            regions.AddRange(Barriers);
            regions.AddRange(Areas);

            return regions;
        }
    }
}
