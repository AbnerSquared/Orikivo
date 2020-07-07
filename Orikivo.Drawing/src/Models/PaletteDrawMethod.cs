namespace Orikivo.Drawing
{
    public enum PaletteDrawMethod
    {
        Ignore = 1, // the palette isn't used.
        Layer = 2, // the palette is referenced on layers
        Force = 4 // the palette is forced onto everything
    }
}
