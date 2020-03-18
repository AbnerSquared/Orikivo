﻿using Orikivo.Drawing;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a timeless region in a <see cref="Sector"/>.
    /// </summary>
    public class Area : Location
    {
        private string _sectorId;

        public override LocationType Type => LocationType.Area;

        /// <summary>
        /// Represents the image that is shown whenever a <see cref="Husk"/> enters this <see cref="Area"/>. If none was specified, the image will be hidden.
        /// </summary>
        public Sprite Exterior { get; set; }

        // Each specified entrance must be touching one side of the specified region.
        /// <summary>
        /// Defines the specific points at which a <see cref="Husk"/> can enter this <see cref="Area"/>.
        /// </summary>
        public List<Vector2> Entrances { get; set; }

        /// <summary>
        /// Represents the collection of buildings that can be entered within this <see cref="Area"/>.
        /// </summary>
        public List<Construct> Constructs { get; set; }

        public List<Structure> Structures { get; set; }

        // TODO: Remove references to NPCs,
        // as they will now be handled outside of land
        /// <summary>
        /// Represents the collection of interactive characters in this <see cref="Area"/>.
        /// </summary>
        public List<Npc> Npcs { get; set; }

        public Construct GetConstruct(string id)
            => Constructs?.FirstOrDefault(x => x.Id == id);

        public override List<Location> GetChildren(bool includeInnerChildren = true)
        {
            var children = new List<Location>();

            if (Constructs?.Count > 0)
                children.AddRange(Constructs);

            return children;
        }
    }
}
