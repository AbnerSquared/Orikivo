using Orikivo.Drawing;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a generic plot of land in a <see cref="World"/>.
    /// </summary>
    public class Region
    {
        public virtual string Id { get; set; }

        public virtual RegionType Subtype => RegionType.Default;

        public string Name { get; set; }

        public RegionF Perimeter { get; set; }

        /// <summary>
        /// Gets a floating-point integer that represents the global x-coordinate of this <see cref="Region"/>.
        /// </summary>
        public float Longitude { get; }

        /// <summary>
        /// Gets a floating-point integer that represents the global y-coordinate of this <see cref="Region"/>.
        /// </summary>
        public float Latitude { get; }
    }
}
