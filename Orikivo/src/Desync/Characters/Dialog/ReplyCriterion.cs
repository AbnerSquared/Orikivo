
using System;

namespace Orikivo.Desync
{
    public class ReplyCriterion
    {
        public Func<Npc, bool> Judge { get; set; }

        public int Priority { get; set; }
    }

}
