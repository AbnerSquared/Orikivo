using Orikivo.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Unstable
{
    public class World
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Sprite Map { get; set; }

        public List<Field> Fields { get; set; }

        public List<Sector> Sectors { get; set; }

        public System.Drawing.SizeF Boundary { get; set; }

        public List<RegionF> Barriers { get; set; }

        /// <summary>
        /// Determines the travel time ratio for this <see cref="World"/>.
        /// </summary>
        public float Scale { get; set; }

        public Sector GetSector(string id)
        {
            return Sectors.First(x => x.Id == id);
        }

        public Field GetField(string id)
            => Fields.First(x => x.Id == id);
    }
}
