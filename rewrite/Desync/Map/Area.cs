using Orikivo.Drawing;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Orikivo.Unstable
{
    /// <summary>
    /// Represents a timeless location in a <see cref="Sector"/>.
    /// </summary>
    public class Area
    {
        public string Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Represents the size and position of the <see cref="Area"/>.
        /// </summary>
        public RegionF Region { get; set; }

        /// <summary>
        /// Represents the image that is shown whenever a <see cref="Husk"/> enters this <see cref="Area"/>. If none was specified, the image will be hidden.
        /// </summary>
        public Sprite Image { get; set; }

        // Each specified entrance must be touching one side of the specified region.
        /// <summary>
        /// Defines the specific points at which a <see cref="Husk"/> can enter this <see cref="Area"/>.
        /// </summary>
        public List<Vector2> Entrances { get; set; }

        /// <summary>
        /// Represents the collection of buildings that can be entered within this <see cref="Area"/>.
        /// </summary>
        public List<Construct> Constructs { get; set; }

        /// <summary>
        /// Represents the collection of interactive characters in this <see cref="Area"/>.
        /// </summary>
        public List<Npc> Npcs { get; set; }

        public Construct GetConstruct(string id)
        {
            return Constructs.First(x => x.Id == id);
        }

    }
}
