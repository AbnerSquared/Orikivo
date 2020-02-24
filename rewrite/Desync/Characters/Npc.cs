using System.Collections.Generic;

namespace Orikivo.Unstable
{
    /// <summary>
    /// Represents a non-playable character in a <see cref="World"/>.
    /// </summary>
    public class Npc
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Personality Personality { get; set; }

        // If this is empty, don't render.
        public NpcSheet Sheet { get; set; }

        // a list of initial relationships with other NPCs.
        public List<Relationship> Relations { get; set; }

        // The NPC will cycle through each routine.
        // The routine takes effect on the first day at which the user awakens
        // the routine progress is then determined based on their starting time in UTC.

        // Likewise, if the routine isn't a daily basis, it starts once the user awakens
        /// <summary>
        /// Represents the <see cref="Npc"/>'s daily tasks. If none is specified, the <see cref="Npc"/> will remain at its starting location.
        /// </summary>
        public Routine Routine { get; set; }
    }
}
