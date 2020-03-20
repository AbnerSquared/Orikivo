using Orikivo.Drawing;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a region in a <see cref="Field"/> that alters the attributes of a <see cref="Husk"/>.
    /// </summary>
    public class Biome : Region
    {
        public TransportType Transport { get; set; }
        
        public BiomeType Type { get; set; }
        
        public override RegionType Subtype => RegionType.Biome;

        /// <summary>
        /// Determines how a <see cref="Husk"/> is affected in this <see cref="Biome"/>.
        /// </summary>
        public Atmosphere Atmosphere { get; set; } 

        public SpawnTable Creatures { get; set; }
    }
}
