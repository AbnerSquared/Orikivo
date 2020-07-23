namespace Arcadia
{
    public enum PaletteDirection
    {
        First = 1,
        Last = 2
    }
    // 9 => GammaGreen :: Glass
    public enum PaletteType // If the palette type contains multiple flags, merge
    {
        Default = 0,
        GammaGreen = 1,
        Crimson = 2,
        Wumpite = 4,
        Glass = 8
    }
}
