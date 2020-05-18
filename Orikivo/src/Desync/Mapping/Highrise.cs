using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Desync
{
    // TODO: Remove Npcs.
    /// <summary>
    /// Represents a <see cref="Construct"/> with multiple stories.
    /// </summary>
    public class Highrise : Construct
    {
        public Highrise()
        {
            Tag = ConstructType.Highrise;
        }
        public List<Floor> Floors { get; set; }

        public Floor GetFloor(int level)
        {
            Floor floor = Floors.First(x => x.Index == level);
            floor.ParentId = Id;

            return floor;
        }

        // TODO: Get rid of NPCs.
        public override List<Npc> Npcs => Floors.Select(x => x.Npcs).Flatten().ToList();

        public override ChildType GetAllowedChildren()
            => ChildType.Floor;

        public override List<Location> GetChildren(bool includeInnerChildren = true)
        {
            var children = base.GetChildren();

            if (Floors?.Count > 0)
                children.AddRange(Floors);

            return children;
        }
    }
}
