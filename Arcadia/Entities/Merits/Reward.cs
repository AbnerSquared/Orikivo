using System.Collections.Generic;

namespace Arcadia
{
    public class Reward
    {
        public Dictionary<string, int> ItemIds { get; set; }
        public long Money { get; set; }
        public long Exp { get; set; }
    }
}