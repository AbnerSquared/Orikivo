using System;

namespace Orikivo.Unstable
{

    public class BoosterData
    {
        public BoosterData(BoosterType type, float rate, TimeSpan? decayLength = null, int? useLimit = null)
        {
            Type = type;
            Rate = rate;

            if (decayLength.HasValue)
                ExpiresOn = DateTime.UtcNow.Add(decayLength.Value);

            if (useLimit.HasValue)
                UsesRemaining = useLimit.Value;
        }

        public BoosterType Type { get; }
        // the rate of the booster
        public float Rate { get; }
        // when the booster was used.
        public DateTime? ExpiresOn { get; }

        // how many times the booster has been activated
        public int? UsesRemaining { get; }
    }
}
