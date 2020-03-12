﻿using System.Collections.Generic;
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
        public Dictionary<DialogueTone, float> ToneImpactRates { get; }
    }

    // how they handle varying tones
    public class PersonalityTones
    {

    }

    // this spectrum represents our core motives
    public class PersonalityAspects
    {
        public MindType Mind { get; set; }
        public EnergyType Energy { get; set; }
        public NatureType Nature { get; set; }
        public TacticType Tactics { get; set; }
        public IdentityType Identity { get; set; }
    }
}
