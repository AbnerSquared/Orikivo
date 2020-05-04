﻿using System.Collections.Generic;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a non-playable character in a <see cref="World"/>.
    /// </summary>
    public class Npc
    {
        public string Id { get; set; }

        public string Name { get; set; }

        // this defines how the NPC thinks
        public Personality Personality { get; set; }

        // this is what the NPC looks like
        public NpcModel Model { get; set; }
        
        // this is who the NPC likes/dislikes initially
        public List<AffinityData> Relations { get; set; }

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
