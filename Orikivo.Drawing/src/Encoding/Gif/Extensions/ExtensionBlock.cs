namespace Orikivo.Drawing.Encoding.Gif
{
    public abstract class ExtensionBlock
    {
        protected const byte INTRODUCER = 0x21; // !
        protected const byte TERMINATOR = 0x00; // NULL

        protected abstract byte Label { get; }
        protected abstract byte? BlockSize { get; }

        protected abstract byte[] GetByteArray();
    }
}
