using System.Collections.Generic;

namespace Orikivo.Unstable
{
    public class Construct // a simple building or such in an area.
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<ConstructLevel> Levels { get; set; }
    }

}
