namespace Orikivo.Drawing.Encoding.Gif
{
    public class ImageDescriptor
    {
        
        ushort LeftOffset; // 2 bytes
        ushort TopOffset; // 2 bytes
        ushort Width; // 2 bytes
        ushort Height; // 2 bytes
        byte Packed;
        // 01234567
        // 765 => bpp - 1
        // 432 => reserved (0)
        // 1 => Interlaced/Sequential image
        // 0 => local/global color map, ignore bits 0-2
    }
}
