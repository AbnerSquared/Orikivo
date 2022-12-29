using System;

namespace Arcadia
{
    /// <summary>
    /// Represents a base modifier for a <see cref="Var"/>.
    /// </summary>
    public class ModifierData
    {
        public ModifierData(long amount, int? useCount = null)
        {
            Amount = amount;
            Rate = 1.0f;
            UsesLeft = useCount;
        }

        public ModifierData(float rate, int? useCount = null)
        {
            Amount = 0;
            Rate = rate;
            UsesLeft = useCount;
        }

        public long Amount { get; }

        public float Rate { get; }

        public int? UsesLeft { get; internal set; }
    }
}
