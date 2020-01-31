using Orikivo.Drawing;
using System.Collections.Generic;

namespace Orikivo.Unstable
{
    public class Field // a dangerous location
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Sprite> Images { get; set; }

        public List<FieldEffect> Effects { get; set; }
        public DiscoveryTable Discoverables { get; set; }
    }
}
