using System;

namespace Arcadia
{
    /// <summary>
    /// Represents a base modifier for a <see cref="Var"/>.
    /// </summary>
    public class ModifierData
    {
        public ModifierData(long amount, TimeSpan duration)
        {
            Amount = amount;
            ExpiresOn = DateTime.UtcNow.Add(duration);
        }

        public ModifierData(float rate, TimeSpan duration)
        {
            Rate = rate;
            ExpiresOn = DateTime.UtcNow.Add(duration);
        }

        public ModifierData(long amount, int useCount)
        {
            Amount = amount;
            UsesLeft = useCount;
        }

        public ModifierData(float rate, int useCount)
        {
            Rate = rate;
            UsesLeft = useCount;
        }

        public ModifierData(long amount, TimeSpan duration, int useCount)
        {
            Amount = amount;
            ExpiresOn = DateTime.UtcNow.Add(duration);
            UsesLeft = useCount;
        }

        public ModifierData(float rate, TimeSpan duration, int useCount)
        {
            Rate = rate;
            ExpiresOn = DateTime.UtcNow.Add(duration);
            UsesLeft = useCount;
        }

        public long Amount { get; }

        public float Rate { get; }

        // TODO: Remove this property
        public DateTime? ExpiresOn { get; }

        public int? UsesLeft { get; internal set; }
    }
}
