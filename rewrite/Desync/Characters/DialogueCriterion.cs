using System.Collections.Generic;

namespace Orikivo.Unstable
{
    /// <summary>
    /// Represents a collection of requirements that need to be met in order for a <see cref="Dialogue"/> to be chosen.
    /// </summary>
    public class DialogueCriterion
    {
        // the minimum that the relationship must be at
        public float MinRelation { get; set; }

        // the max possible value that this relationship can be at
        public float MaxRelation { get; set; }

        // the archetype that the npc speaking needs to have in order to choose this dialogue.
        public Archetype PreferredArchetype { get; set; }

        // a list of flags a husk needs to have in order to use this dialogue.
        public List<string> Flags { get; set; }
    }

}
