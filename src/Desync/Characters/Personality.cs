using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents the entire personality of an <see cref="Npc"/> that determines how they communicate.
    /// </summary>
    public class Personality
    {
        public Archetype Archetype { get; set; }

        // a multiplier set for each dialogue tone that affects their relations.
        public Dictionary<DialogueTone, float> ToneImpactRates { get; }
    }
}
