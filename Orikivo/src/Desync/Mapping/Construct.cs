using Orikivo.Drawing;
using System.Collections.Generic;

namespace Orikivo.Desync
{
    // TODO: Implement Schedules and separate NPCs from locations.
    /// <summary>
    /// Represents an accessable building in a <see cref="Location"/>.
    /// </summary>
    public class Construct : Location
    {
        public override LocationType Type => LocationType.Construct;

        public Sprite Image { get; set; }
        public ConstructType Tag { get; set; }

        public virtual List<Npc> Npcs { get; set; } = new List<Npc>();

        /// <summary>
        /// Represents the availability of this <see cref="Construct"/>. If none is specified, the <see cref="Construct"/> will always be available. (unimplemented)
        /// </summary>
        public virtual Schedule Schedule { get => throw new System.NotImplementedException(); }
    }
}
