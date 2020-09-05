using System;

namespace Orikivo.Desync
{
    public class DialogResult
    {
        /// <summary>
        /// Represents an action to be applied to a Husk and HuskBrain after the completion of this dialog.
        /// </summary>
        public Action<Husk, HuskBrain> Post { get; set; }

        /// <summary>
        /// Represents the value that an affinity between a character is changed by.
        /// </summary>
        public float Impact { get; set; }
    }
}
