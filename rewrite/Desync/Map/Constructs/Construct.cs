using System.Collections.Generic;

namespace Orikivo.Unstable
{
    public class Construct // a simple building or such in an area.
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<ConstructLayer> Layers { get; set; }

        // create a husk reference at this location
        public Husk InitializeHusk()
        {
            // TODO: This is supposed to reference the world, sector, area, and construct
            return new Husk(Id);
        }
    }

}
