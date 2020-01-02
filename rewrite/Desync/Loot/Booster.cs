using System;

namespace Orikivo.Unstable
{
    public class Booster
    {
        public BoosterType Type { get; set; }
        public TimeSpan? DecayLength { get; set; }
        public int? UseLimit { get; set; }
        public float Rate { get; set; }
    }
}
