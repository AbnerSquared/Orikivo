using Orikivo.Drawing;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Orikivo.Unstable
{
    public class Sector
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Vector2 Position { get; set; }

        public SectorScale Scale { get; set; }

        // This is used to determine how to enter the sector.
        public Vector2 Entrance { get; set; }

        public List<Area> Areas { get; set; } // All areas must be conjoined

        public Sprite Map { get; set; }

        // a list of other decoratives in a sector
        // NOTE: structures cannot intersect areas.
        public List<Structure> Structures { get; set; }

        // a list of npcs at a specified location.
        public List<(Vector2 Position, Npc Npc)> Npcs { get; set; }

        public Area GetArea(string id)
        {
            return Areas.First(x => x.Id == id);
        }
    }
}
