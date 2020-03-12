using Orikivo.Drawing;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a impassable region in a <see cref="Location"/>.
    /// </summary>
    public class Barrier
    {
        /// <summary>
        /// Represents the size and position of this <see cref="Barrier"/>.
        /// </summary>
        public RegionF Region { get; set; }

        /// <summary>
        /// Defines how this <see cref="Barrier"/> blocks a <see cref="Husk"/>.
        /// </summary>
        public BarrierTag Tag { get; set; }

        /// <summary>
        /// The criteria needed to be able to bypass this <see cref="Barrier"/>. If none is set, this <see cref="Barrier"/> is always active.
        /// </summary>
        public LocationCriteria Criteria { get; }
    }
}
