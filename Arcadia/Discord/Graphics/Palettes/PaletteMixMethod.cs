namespace Arcadia.Graphics
{
    public enum PaletteMixMethod
    {
        /// <summary>
        /// Blends both palettes as a 50% mixture.
        /// </summary>
        Blend = 1,

        /// <summary>
        /// Transitions from the primary to the secondary palette.
        /// </summary>
        Smear = 2
    }
}
