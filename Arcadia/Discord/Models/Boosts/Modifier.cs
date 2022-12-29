using System;

namespace Arcadia
{
    /// <summary>
    /// Represents a base modifier for an <see cref="ItemProperty"/>.
    /// </summary>
    public class Modifier
    {
        // The property ID that this modifier can be applied to.
        public string PropertyId { get; set; }

        // If this modifier can be placed on empty property definitions
        public bool AllowOnEmpty { get; set; } = true;

        // The value to append onto this property
        public long Value { get; set; }

        // The rate to modify this property by
        public float Rate { get; set; }

        // The amount of times this modifier is used before it is removed. If use count is unspecified, the value is permanently applied to the base value.
        public int? UseCount { get; set; }
    }
}
