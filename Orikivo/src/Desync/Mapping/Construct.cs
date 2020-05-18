using Orikivo.Drawing;
using System.Collections.Generic;

namespace Orikivo.Desync
{
    // TODO: Implement Schedules and separate NPCs from locations.
    /// <summary>
    /// Represents an accessible building in a <see cref="Location"/>.
    /// </summary>
    public class Construct : Location
    {
        public override LocationType Type => LocationType.Construct;

        /// <summary>
        /// Represents the background that is used for an <see cref="Npc"/> when talking.
        /// </summary>
        public Sprite Interior { get; set; }

        public ConstructType Tag { get; set; } = ConstructType.Default;

        public virtual List<Npc> Npcs { get; set; } = new List<Npc>();

        /// <summary>
        /// Represents the availability of this <see cref="Construct"/>. If none is specified, the <see cref="Construct"/> will always be available. (unimplemented)
        /// </summary>
        public virtual Schedule Schedule { get => throw new System.NotImplementedException(); }
    }
}
