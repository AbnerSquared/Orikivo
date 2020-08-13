namespace Orikivo.Drawing
{
    public enum DrawableColorHandling
    {
        /// <summary>
        /// Ignores overriding the colors on each <see cref="DrawableLayer"/>.
        /// </summary>
        Ignore = 1,

        /// <summary>
        /// Attempts to replace the colors on each <see cref="DrawableLayer"/> using a color map reference.
        /// </summary>
        Map = 2,

        /// <summary>
        /// Forces the colors on each <see cref="DrawableLayer"/> to the specified <see cref="GammaPalette"/>.
        /// </summary>
        Force = 4
    }
}
