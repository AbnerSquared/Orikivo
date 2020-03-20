namespace Orikivo.Drawing.Encoding.Gif
{
    public abstract class Extension
    {

    }
    // this is the base of an extension
    // this could be an abstract class, with its overridable values.
    public class ExtensionBlock
    {
        byte Marker = (byte)'!'; // always !, marks an extension 0x21

        // AKA the label
        byte Id; // 1 byte, the identity of this extension block, range from 00 to FF
        ushort ByteCount; // 2 bytes, defines how many bytes are being defined
        byte[] Data; // n bytes, defined by ByteCount.
        byte Terminator = 0x00; // zero byte char, closes the extension block
    }
}
