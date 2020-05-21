namespace Orikivo.Drawing.Encoding.Gif
{
    // this has a maximum of 8 unique possible values that can be set with 3 bits
    public enum DisposalMethod : byte
    {
        // ---432-- // These are the bits that are utilized
        Unspecified = 0, // 00000000 // 00
        DoNotDispose = 4, // 00000100 // 01
        RestoreBackgroundColor = 8, // 00001000 // 10
        RestorePrevious = 12 // 00001100 // 11
    }
}
