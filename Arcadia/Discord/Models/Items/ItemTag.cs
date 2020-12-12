using System;

namespace Arcadia
{
    /// <summary>
    /// Defines a collection of common traits for an <see cref="Item"/>.
    /// </summary>
    [Flags]
    public enum ItemTag
    {
        /// <summary>
        /// Specifies that this item can be used.
        /// </summary>
        Usable = 1,

        /// <summary>
        /// Specifies that this item can be equipped or held for additional benefits.
        /// </summary>
        Equipment = 2,

        /// <summary>
        /// Specifies that this items stores other item instances.
        /// </summary>
        Container = 4,

        /// <summary>
        /// Specifies that this item is a crafting ingredient.
        /// </summary>
        Material = 8,

        /// <summary>
        /// Specifies that this item modifies how a card is displayed.
        /// </summary>
        Decorator = 16,

        /// <summary>
        /// Specifies that this item can be disposed.
        /// </summary>
        Disposable = 32,

        /// <summary>
        /// Specifies that this item can be sealed.
        /// </summary>
        Sealable = 64,

        /// <summary>
        /// Specifies that this item can be cloned.
        /// </summary>
        Cloneable = 128,

        /// <summary>
        /// Specifies that this item can be renamed.
        /// </summary>
        Renamable = 256,

        /// <summary>
        /// Specifies that this item can be modified.
        /// </summary>
        Modifiable = 512,

        /// <summary>
        /// Specifies that this item can modify attributes.
        /// </summary>
        Modifier = 1024,

        /// <summary>
        /// Specifies that this item can be ordered from the catalog.
        /// </summary>
        Orderable = 2048

        // Specifies that this item can be traded.
        // Tradable = 4096

        // Specifies that this item can be stacked.
        // Stackable = 8192
    }
}
