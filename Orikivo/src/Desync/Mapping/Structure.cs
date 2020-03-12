using Orikivo.Drawing;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a external construction in a <see cref="Location"/>.
    /// </summary>
    public class Structure : Location
    {
        public override LocationType Type => LocationType.Structure;

        /// <summary>
        /// Defines the specific type of <see cref="Structure"/> this is.
        /// </summary>
        public StructureType Tag { get; set; }

        /// <summary>
        /// Represents the position and size of this <see cref="Structure"/> in relation to its parent <see cref="Location"/>.
        /// </summary>
        public RegionF Region { get; set; }
    }
}
