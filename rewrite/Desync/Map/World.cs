using System.Collections.Generic;

namespace Orikivo.Unstable
{
    public class World

    {
        public string MapImageUrl { get; set; }
        public string Name { get; set; }
        public List<Sector> Sectors { get; set; }
        // sectors are safe places, anything else could be called a Field
        public List<Field> Fields { get; set; }
    }
}
