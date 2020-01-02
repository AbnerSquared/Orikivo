using System.Collections.Generic;

namespace Orikivo.Unstable
{
    public class Sector
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public List<Area> Areas { get; set; }
    }
}
