using System;
using System.Collections.Generic;

namespace Arcadia
{
    /// <summary>
    /// Represents a custom property for an <see cref="Item"/>.
    /// </summary>
    public class ItemProperty
    {
        public string Id { get; set; }

        public ItemPropertyType Type { get; set; } = ItemPropertyType.Static;

        // If true, modifiers can be applied to this item property
        public bool CanModify { get; set; } = false;

        public bool ModifyBaseValue { get; set; } = false;

        public long? DefaultValue { get; set; }

        public DateTime? ExpiresOn { get; set; }
    }
}
