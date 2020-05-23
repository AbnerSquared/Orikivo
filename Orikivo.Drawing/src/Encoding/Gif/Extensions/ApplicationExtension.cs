namespace Orikivo.Drawing.Encoding.Gif
{
    // 14 bytes in length.
    public class ApplicationExtension : ExtensionBlock
    {
        // The characters must be 8 printable ASCII characters.
        private const byte MAX_ID_LENGTH = 8;
        private const byte MAX_CODE_LENGTH = 3;
        private const string DEFAULT_ID = "NETSCAPE";
        private const string DEFAULT_CODE = "2.0";

        protected override byte Label => 0xFF;
        protected override byte? BlockSize => 11;

        // Byte 1-8
        // Application Identifier
        // This is only a max of 8 ASCII characters
        // The default code will be NETSCAPE
        public string Identifier; // A default is NETSCAPE

        // Byte 9-11
        // Application Authentication Code
        // This is only a max of 3 ASCII characters
        // The default code will be 2.0
        public string AuthenticationCode;

        // N
        // Application Data
        // An optional property that can contain extra application data
        // that can be inserted.
        public DataBlock[] ApplicationData;

        protected override byte[] GetByteArray()
        {
            throw new System.NotImplementedException();
        }
    }

}
