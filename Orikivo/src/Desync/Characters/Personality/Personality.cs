using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents the entire personality of an <see cref="Npc"/> that determines how they communicate.
    /// </summary>
    public class Personality
    {
        public PersonalityAspects Aspects { get; set; }
        
        // other factors;
        // patience
        // emotional whiplash
        // ability to remain calm in danger
        // emotional events
        
        public Archetype Archetype { get; set; }

        // a multiplier set for each dialogue tone that affects their relations.
        public Dictionary<DialogTone, float> ToneImpactRates { get; }
    }
}
