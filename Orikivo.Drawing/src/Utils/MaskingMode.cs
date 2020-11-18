namespace Orikivo.Drawing
{
    /// <summary>
    /// Defines the masking mode for an opacity mask.
    /// </summary>
    public enum MaskingMode
    {
        /// <summary>
        /// The opacity mask overrides any existing opacity values.
        /// </summary>
        Set = 1,

        /// <summary>
        /// Clamps the opacity mask to the maximum existing opacity value at each pixel.
        /// </summary>
        Clamp = 2
    }
}
