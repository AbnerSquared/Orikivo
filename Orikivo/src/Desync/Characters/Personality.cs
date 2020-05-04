using System;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents the brain of a character that determines how they communicate.
    /// </summary>
    public class Personality
    {
        // loads a personality based on its bitwise flag.
        public static Personality FromBitwiseFlag(int bits)
        {
            throw new NotImplementedException();
        }

        public MindType Mind { get; set; }
        public EnergyType Energy { get; set; }
        public NatureType Nature { get; set; }
        public TacticType Tactics { get; set; }
        public IdentityType Identity { get; set; }

        public int GetBitwiseFlag()
        {
            // 0x00000000
            int bits = 0b00000;
            
            if ((int)Mind == 1)
                bits |= 0b10000;

            if ((int)Energy == 1)
                bits |= 0b01000;

            if ((int)Nature == 1)
                bits |= 0b00100;

            if ((int)Tactics == 1)
                bits |= 0b00010;

            if ((int)Identity == 1)
                bits |= 0b00001;

            return bits;
        }
    }
}
