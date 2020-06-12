using System.Collections.Generic;

namespace Arcadia.Old
{
    public class GameTabProperties
    {
        public string Name { get; set; }
        public List<Element> Elements { get; set; } = new List<Element>();
        public int? Capacity { get; set; }
        public List<ElementGroup> Groups { get; set; } = new List<ElementGroup>();

    }
}
