using Orikivo.Drawing;
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
        public override LocationType Type => LocationType.Area;

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
        public List<Character> Npcs { get; set; }

        public Construct GetConstruct(string id)
            => Constructs?.FirstOrDefault(x => x.Id == id);

        public override ChildType GetAllowedChildren()
            => ChildType.Construct | ChildType.Structure;

        public override List<Location> GetChildren(bool includeInnerChildren = true)
        {
            var children = new List<Location>();

            if (Constructs?.Count > 0)
            {
                children.AddRange(Constructs);

                if (includeInnerChildren)
                {
                    foreach(Construct construct in Constructs)
                    {
                        if (construct.Tag == ConstructType.Highrise)
                            children.AddRange(construct.GetChildren());
                    }
                }
            }

            return children;
        }

        public override List<Region> GetRegions()
        {
            var regions = new List<Region>();

            regions.AddRange(GetChildren(false));

            if (Structures?.Count > 0)
                regions.AddRange(Structures);

            return regions;
        }
        
    }
}
