namespace Orikivo.Drawing
{
    public enum MaskingMode
    {
        // No matter what, the new mask replaces the current mask's opacity
        Set = 1,
        // The maximum opacity that can be set is clamped to the current mask's opacity
        Clamp = 2
    }
}
