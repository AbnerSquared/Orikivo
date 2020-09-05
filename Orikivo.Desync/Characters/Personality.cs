using System;
using System.Collections.Generic;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents the brain of a <see cref="Character"/> that determines how they communicate.
    /// </summary>
    public class Personality
    {
        // loads a personality based on its bitwise flag.
        public static Personality FromBitwiseFlag(int bits)
        {
            var personality = new Personality
            {
                // 0b11111 & 0b10000 => 0b10000
                Mind = (MindType)(bits & 0b10000),
                Energy = (EnergyType)(bits & 0b01000),
                Nature = (NatureType)(bits & 0b00100),
                Tactics = (TacticType)(bits & 0b00010),
                Identity = (IdentityType)(bits & 0b00001)
            };

            return personality;
        }

        private static int GetBitwiseFlag(MindType mind, EnergyType energy, NatureType nature, TacticType tactics, IdentityType identity)
        {
            // 0x00000000
            int bits = 0b00000;
            
            bits |= (int)mind
                 | (int)nature
                 | (int)energy
                 | (int)tactics
                 | (int)identity;

            return bits;
        }

        public Personality() { }

        public Personality(MindType mind, EnergyType energy, NatureType nature, TacticType tactics, IdentityType identity)
        {
            Mind = mind;
            Energy = energy;
            Nature = nature;
            Tactics = tactics;
            Identity = identity;
        }

        public Personality(int bits)
        {
            Mind = (MindType)(bits & 0b10000);
            Energy = (EnergyType)(bits & 0b01000);
            Nature = (NatureType)(bits & 0b00100);
            Tactics = (TacticType)(bits & 0b00010);
            Identity = (IdentityType)(bits & 0b00001);
        }

        public MindType Mind { get; set; }
        public EnergyType Energy { get; set; }
        public NatureType Nature { get; set; }
        public TacticType Tactics { get; set; }
        public IdentityType Identity { get; set; }

        // a list of modifiers that this personality alters;
        // can be left null
        public List<ToneModifier> Modifiers { get; set; }

        public int GetBitwiseFlag()
            => GetBitwiseFlag(Mind, Energy, Nature, Tactics, Identity);
    }
}
