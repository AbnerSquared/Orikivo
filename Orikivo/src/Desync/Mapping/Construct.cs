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
        public Construct()
        {
            CanUseDecor = true;
        }

        public Construct(string id, string name) : this()
        {
            Id = id;
            Name = name;
        }

        public override LocationType Type => LocationType.Construct;

        protected bool CanUseDecor { get; set; }

        /// <summary>
        /// Represents the background that is used for an <see cref="Character"/> when talking.
        /// </summary>
        public Sprite Interior { get; set; }

        public List<Decor> Decors { get; set; } = new List<Decor>();

        public ConstructType Tag { get; set; } = ConstructType.Default;

        public virtual List<Character> Npcs { get; set; } = new List<Character>();

        /// <summary>
        /// Represents the availability of this <see cref="Construct"/>. If none is specified, the <see cref="Construct"/> will always be available. (unimplemented)
        /// </summary>
        public virtual Schedule Schedule { get => throw new System.NotImplementedException(); }
    }
}
