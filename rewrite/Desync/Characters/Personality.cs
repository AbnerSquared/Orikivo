using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Unstable
{
    /// <summary>
    /// Represents the entire personality of an <see cref="Npc"/> that determines how they communicate.
    /// </summary>
    public class Personality
    {
        public PersonalityArchetype Archetype { get; set; }

        // a multiplier set for each dialogue tone that affects their relations.
        public Dictionary<DialogueTone, float> ImpactRates { get; }
    }
}
