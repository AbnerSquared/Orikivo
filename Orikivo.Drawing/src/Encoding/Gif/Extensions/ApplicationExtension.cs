namespace Orikivo.Drawing.Encoding.Gif
{
    // 14 bytes in length.
    public class ApplicationExtension
    {
        byte Introducer;
        byte Label; // FF == Application Extension
        byte BlockSize; // always 0B, number of bytes in ID and AuthCode.
        char[] Identifier; // 8 chars // METSCAPE
        byte[] AuthenticationCode; // 3 bytes // 2.0
        SubBlock[] ApplicationData; 
        byte Terminator = 0x00;
    }
}
