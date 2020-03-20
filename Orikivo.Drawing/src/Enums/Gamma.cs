namespace Orikivo.Drawing
{
    /// <summary>
    /// Represents a level of brightness for a <see cref="GammaPalette"/>.
    /// </summary>
    public enum Gamma
    {
        /// <summary>
        /// The darkest <see cref="GammaColor"/> within a <see cref="GammaPalette"/>.
        /// </summary>
        Min = 0,

        Dimmer = 1,

        Dim = 2,

        StandardDim = 3,

        Standard = 4,

        Bright = 5,

        Brighter = 6,

        /// <summary>
        /// The brightest <see cref="GammaColor"/> within a <see cref="GammaPalette"/>.
        /// </summary>
        Max = 7
    }
}
