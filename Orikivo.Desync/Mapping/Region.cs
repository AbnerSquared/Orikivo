using Orikivo.Drawing;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a generic plot of land in a <see cref="World"/>.
    /// </summary>
    public class Region
    {
        /// <summary>
        /// Represents the internal name of this <see cref="Region"/>.
        /// </summary>
        public virtual string Id { get; set; }

        /// <summary>
        /// Represents the base generic type of this <see cref="Region"/>.
        /// </summary>
        public virtual RegionType Subtype => RegionType.Default;

        /// <summary>
        /// Represents the name of this <see cref="Region"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Represents the size and relative coordinates of this <see cref="Region"/>.
        /// </summary>
        public RegionF Shape { get; set; }

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
