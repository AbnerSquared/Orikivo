namespace Arcadia.Graphics
{
    /// <summary>
    /// Defines how a component is filled.
    /// </summary>
    public enum FillMode
    {
        /// <summary>
        /// Components are not rendered.
        /// </summary>
        None = 0,

        /// <summary>
        /// Components are rendered as a solid color. [Requires Palette, Primary]
        /// </summary>
        Solid = 1,

        /// <summary>
        /// Components are rendered as a two-tone fillable. [Requires Palette, Primary, Secondary, Direction]
        /// </summary>
        Bar = 2,

        /// <summary>
        /// Components are rendered as a gradient. [Requires Palette, Direction]
        /// </summary>
        Gradient = 3,

        /// <summary>
        /// Components are rendered with reference to a palette. [Requires Palette]
        /// </summary>
        Reference = 4
    }
}
