namespace Orikivo.Desync
{
    public class Atmosphere
    {
        /// <summary>
        /// Represents a value that affects how far a <see cref="Husk"/> can see into the distance.
        /// </summary>
        public float ViewDistance { get; set; }

        /// <summary>
        /// Represents a value that affects how fast a <see cref="Husk"/> can travel in this region.
        /// </summary>
        public float TravelSpeed { get; set; }

        /// <summary>
        /// Represents a value that determines how poisonous the <see cref="Biome"/> is.
        /// </summary>
        public float ExposureStrength { get; set; }

        /// <summary>
        /// Represents a value that affects how often a <see cref="Creature"/> can spawn.
        /// </summary>
        public float SpawnRate { get; set; }

        /// <summary>
        /// Represents a value that affects how often a relic can appear.
        /// </summary>
        public float RelicSpawnRate { get; set; }

        /// <summary>
        /// Represents a value that affects how often a mineral can appear.
        /// </summary>
        public float MineralSpawnRate { get; set; }
    }
}
