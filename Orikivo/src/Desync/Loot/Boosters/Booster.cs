using System;

namespace Orikivo.Desync
{
    public class Booster
    {
        public BoosterType Type { get; set; }
        public TimeSpan? Decay { get; set; }
        public int? UseLimit { get; set; }
        public float Rate { get; set; }
    }
}
