namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a interactive <see cref="Region"/> in a <see cref="Location"/>.
    /// </summary>
    public class Structure : Region
    {
        public override RegionType Subtype => RegionType.Biome;
        /// <summary>
        /// Defines the specific type of <see cref="Structure"/> this is.
        /// </summary>
        public StructureType Type { get; set; }
    }
}
