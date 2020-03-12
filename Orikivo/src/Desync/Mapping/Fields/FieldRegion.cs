using Orikivo.Drawing;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a region in a <see cref="Field"/> that alters the attributes of a <see cref="Husk"/>.
    /// </summary>
    public class FieldRegion : LocationRegion
    {
        /// <summary>
        /// Represents a value that affects how far a <see cref="Husk"/> can see into the distance.
        /// </summary>
        public float Sight { get; set; }

        /// <summary>
        /// Represents a value that affects how fast a <see cref="Husk"/> can travel in this region.
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// Represents a value that affects how often a <see cref="Creature"/> can spawn.
        /// </summary>
        public float SpawnRate { get; set;  }

        /// <summary>
        /// Represents a value that affects how often a relic can appear.
        /// </summary>
        public float RelicRate { get; set; }

        /// <summary>
        /// Represents a value that affects how often a mineral can appear.
        /// </summary>
        public float MineralRate { get; set; }    
    }
}
