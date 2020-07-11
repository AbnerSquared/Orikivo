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
            CanUseDecor = false;
        }

        public Highrise(string id, string name) : this()
        {
            Id = id;
            Name = name;
        }


        private List<Floor> _floors = new List<Floor>();

        public List<Floor> Floors
        {
            get => _floors;
            set
            {
                if (value?.Count == 0)
                {
                    _floors.Clear();
                    return;
                }

                foreach (Floor floor in value)
                {
                    floor.ParentId = Id;
                }
            }
        }

        public Floor GetFloor(int level)
        {
            Floor floor = Floors.First(x => x.Index == level);
            floor.ParentId = Id;

            return floor;
        }

        // TODO: Get rid of NPCs.
        public override List<Character> Npcs => Floors.Select(x => x.Npcs).Flatten().ToList();

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
