
using System;

namespace Orikivo.Desync
{
    public class ReplyCriterion
    {
        public Func<Character, bool> Judge { get; set; }

        public int Priority { get; set; }
    }

}
