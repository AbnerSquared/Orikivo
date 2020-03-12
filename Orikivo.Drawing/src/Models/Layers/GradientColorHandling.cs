namespace Orikivo.Drawing
{
    public enum GradientColorHandling
    {
        Blend = 1, // blends the in-between colors together
        Snap = 2, // goes to the closest color
        Dither = 3 // blends the two colors differences as pixels
    }
}
