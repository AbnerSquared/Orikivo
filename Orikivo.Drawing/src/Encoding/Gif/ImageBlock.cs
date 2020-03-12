namespace Orikivo.Drawing.Encoding.Gif
{
    public class ImageBlock
    {
        byte Separator = (byte)',';
        ImageDescriptor LocalImageDescriptor { get; }
        ColorTable[] LocalColorTable { get; } // 256 colors max
        byte[] ImageData { get; }
    }
}
