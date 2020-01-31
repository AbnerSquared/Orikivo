using Orikivo.Drawing;
using System.Collections.Generic;
using System.Drawing;

namespace Orikivo.Unstable
{
    public class Area
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Construct> Constructs { get; set; }

        public List<Sprite> Images { get; set; }

        public Point Position { get; set; }

        public List<WorldEvent> Events { get; set; }
    }
}
