namespace Orikivo.Drawing.Encoding.Gif
{
    // This is always used at the start of a GIF
    // The version can either be set to 87a, or 89a
    public class Header
    {
        private const string SIGNATURE = "GIF";
        private const string VERSION = "89a"; // or 87a


        private byte[] GetByteArray()
        {
            var bytes = new byte[6];

            return bytes;
        }
    }
}
