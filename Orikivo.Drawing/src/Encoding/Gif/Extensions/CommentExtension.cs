namespace Orikivo.Drawing.Encoding.Gif
{
    // varies from 5 to 259 bytes in length.
    public class CommentExtension
    {
        byte Introducer;
        byte Label; // FE == Identity of Comment Extension
        SubBlock CommentData;
        byte Terminator = 0x00;
    }
}
