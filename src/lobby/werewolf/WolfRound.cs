using System.Collections.Generic;

namespace Orikivo
{
    public class WereDay
    {
        public WereDay()
        {
            Events = new List<string>();
        }

        public List<string> Events { get; }
    }
}
