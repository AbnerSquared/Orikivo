using System;

namespace Arcadia
{
    /// <summary>
    /// Represents a custom property for an <see cref="Item"/>.
    /// </summary>
    public class ItemProperty
    {
        public string Id { get; set; }

        public ItemPropertyType Type { get; set; }

        public long? DefaultValue { get; set; }

        public DateTime? ExpiresOn { get; set; }

        public int? Durability { get; set; }
    }
}
