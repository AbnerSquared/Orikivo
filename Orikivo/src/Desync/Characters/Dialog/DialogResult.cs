using System;

namespace Orikivo.Desync
{
    public class DialogResult
    {
        // this method updates the husk and its brain accordingly.
        public Action<Husk, HuskBrain> Handler { get; set; }

        // changes the relationship with this NPC by this.
        public float Impact { get; set; }
    }
}
