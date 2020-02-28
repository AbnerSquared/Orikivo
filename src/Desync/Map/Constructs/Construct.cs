using Orikivo.Drawing;
using System.Collections.Generic;

namespace Orikivo.Desync
{
    // TODO: Implement Schedules and separate NPCs from locations.
    /// <summary>
    /// Represents a building in an <see cref="Area"/>.
    /// </summary>
    public class Construct
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public Sprite Image { get; set; }
        public ConstructTag Tag { get; set; }

        public virtual List<Npc> Npcs { get; set; } = new List<Npc>();

        /// <summary>
        /// Represents the availability of this <see cref="Construct"/>. If none is specified, the <see cref="Construct"/> will always be available. (unimplemented)
        /// </summary>
        public virtual Schedule Schedule { get => throw new System.NotImplementedException(); }
    }
}
