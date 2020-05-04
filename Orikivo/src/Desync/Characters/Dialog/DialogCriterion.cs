
using System;

namespace Orikivo.Desync
{

    public class DialogCriterion
    {
        // determines the criterion.
        // checks the npc and the husk/brain to see if this dialog is used.
        public Func<Npc, Husk, HuskBrain, bool> Judge { get; set; }

        // if two criterion have the same priority, and both pass, one is chosen at random.
        public int Priority { get; set; }
    }

}
