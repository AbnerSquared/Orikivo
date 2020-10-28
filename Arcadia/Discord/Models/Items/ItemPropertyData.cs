using System.Collections.Generic;

namespace Arcadia
{
    /// <summary>
    /// Represents data for an <see cref="ItemPropertyData"/>.
    /// </summary>
    public class ItemPropertyData
    {
        internal ItemPropertyData(long value, List<ModifierData> modifiers)
        {
            Value = value;
            Modifiers = modifiers;
        }

        /// <summary>
        /// Represents the current value of this <see cref="ItemPropertyData"/>.
        /// </summary>
        public long Value { get; internal set; }

        // EX: +3 Attack (20 uses)
        /// <summary>
        /// Specifies a collection of active modifiers for this <see cref="ItemPropertyData"/>.
        /// </summary>
        public List<ModifierData> Modifiers { get; }
    }
}
