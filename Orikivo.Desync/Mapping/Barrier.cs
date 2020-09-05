using Orikivo.Drawing;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents an impassable <see cref="Region"/> in a <see cref="Location"/>.
    /// </summary>
    public class Barrier : Region
    {
        public override RegionType Subtype => RegionType.Barrier;

        /// <summary>
        /// Defines how this <see cref="Barrier"/> blocks a <see cref="Husk"/>.
        /// </summary>
        public BarrierTag Tag { get; set; }

        // determines the transportation methods that this barrier blocks.
        public TransportType Transport { get; set; }

        /// <summary>
        /// The criteria needed to be able to bypass this <see cref="Barrier"/>. If none is set, this <see cref="Barrier"/> is always active.
        /// </summary>
        public UnlockCriteria Criteria { get; }
    }
}
