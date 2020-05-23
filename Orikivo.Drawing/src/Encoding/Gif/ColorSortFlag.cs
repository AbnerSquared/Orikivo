namespace Orikivo.Drawing.Encoding.Gif
{
    public enum ColorSortFlag : byte
    {
        // bit 4 is the only bit that utilizes this flag
        Unordered = 0, // 00000000
        DecreasingFrequency = 16 // 00010000
    }
}
