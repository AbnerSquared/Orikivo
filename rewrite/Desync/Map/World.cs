using Orikivo.Drawing;
using System.Collections.Generic;

namespace Orikivo.Unstable
{
    public class World

    {
        public string Id { get; set; }
        public Sprite Map { get; set; }
        
        public string Name { get; set; }
        public List<Sector> Sectors { get; set; }
        // sectors are safe places, anything else could be called a Field
        public List<Field> Fields { get; set; }
    }
}
