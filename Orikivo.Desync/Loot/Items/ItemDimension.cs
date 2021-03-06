﻿namespace Orikivo.Desync
{
    // Orikivo-based custom item property
    /// <summary>
    /// Defines the explicit dimension an <see cref="Item"/> was meant for.
    /// </summary>
    public enum ItemDimension
    {
        /// <summary>
        /// Defines the <see cref="Item"/> as a physical object meant for a <see cref="Husk"/>.
        /// </summary>
        Physical = 1,

        /// <summary>
        /// Defines the <see cref="Item"/> as a digital object meant for a <see cref="User"/>.
        /// </summary>
        Digital = 2
    }
}
