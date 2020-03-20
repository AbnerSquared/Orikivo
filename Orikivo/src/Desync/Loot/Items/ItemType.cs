namespace Orikivo.Desync
{
    /// <summary>
    /// Defines the explicit type an <see cref="Item"/> was meant for.
    /// </summary>
    public enum ItemType
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
