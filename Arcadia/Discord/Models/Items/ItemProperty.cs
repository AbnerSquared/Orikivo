namespace Arcadia
{
    /// <summary>
    /// Represents a custom property for an <see cref="Item"/>.
    /// </summary>
    public class ItemProperty
    {
        /// <summary>
        /// Represents the unique identifier for this <see cref="ItemProperty"/>.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Specifies the type of this <see cref="ItemProperty"/>.
        /// </summary>
        public ItemPropertyType Type { get; set; } = ItemPropertyType.Static;

        /// <summary>
        /// When true, allows a <see cref="Modifier"/> to be applied to this <see cref="ItemProperty"/>.
        /// </summary>
        public bool CanModify { get; set; } = false;

        /// <summary>
        /// When true, forces a <see cref="Modifier"/> to use the default specified value when modifying using a rate.
        /// </summary>
        public bool ModifyBaseValue { get; set; } = false;

        /// <summary>
        /// Represents the default value for this <see cref="ItemProperty"/>.
        /// </summary>
        public long? DefaultValue { get; set; }
    }
}
