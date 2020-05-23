namespace Orikivo.Drawing.Encoding.Gif
{
    // varies from 5 to 259 bytes in length.
    public class CommentExtension : ExtensionBlock
    {
        protected override byte Label => 0xFE;

        // this can be left unspecified for extensions
        protected override byte? BlockSize => null;

        public DataBlock[] CommentData;

        protected override byte[] GetByteArray()
        {
            throw new System.NotImplementedException();
        }
    }
}
