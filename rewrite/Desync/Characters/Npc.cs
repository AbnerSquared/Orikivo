using System.Collections.Generic;

namespace Orikivo.Unstable
{
    public class Npc
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Personality Personality { get; set; }

        // a list of initial relationships with other NPCs.
        public List<Relationship> Relations { get; set; }

    }
}
