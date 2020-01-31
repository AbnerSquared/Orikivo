using Orikivo.Drawing;
using System.Collections.Generic;
using System.Drawing;

namespace Orikivo.Unstable
{
    public class Sector
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Sprite Map { get; set; }
        public List<Area> Areas { get; set; }
    
        public Point Position { get; set; }
    }
}
