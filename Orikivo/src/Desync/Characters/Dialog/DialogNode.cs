using System;

namespace Orikivo.Desync
{
    /*
             // tailors this dialog to people that have these flags
        // flags are markers that a player has completed a specific objective or storyline.
        List<string> Flags { get; set; }
         
         
         */

    // this stores a dialogue that has multiple sentences
    public class DialogNode
    {
        public string Entry { get; set; }

        // if none is set, default to Dialogue.Duration
        public TimeSpan Duration { get; set; }
    }

}
