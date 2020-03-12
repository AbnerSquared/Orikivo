namespace Orikivo.Drawing.Encoding.Gif
{
    public class Header
    {
        // Signature: GIF, 3 bytes (ALWAYS KEEP)
        byte[] Signature = { (byte) 'G', (byte) 'I', (byte) 'F' };
        // Version: 87a || 89a, 3 bytes (ALWAYS KEEP)
        byte[] Version = { (byte)'8', (byte)'7', (byte)'a' };
        
    }
}
