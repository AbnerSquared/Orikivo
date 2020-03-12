using Orikivo.Drawing;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a generic distinguishable region in a <see cref="Location"/>.
    /// </summary>
    public class LocationRegion
    {
        public RegionF Region { get; set; }
        public string Name { get; set; }
    }
}
