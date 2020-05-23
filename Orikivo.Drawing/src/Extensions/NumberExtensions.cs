namespace Orikivo.Drawing
{
    public static class NumberExtensions
    {
        public static byte[] ToBytes(this ushort value)
        {
            var bytes = new byte[2];

            bytes[0] = (byte) (value & 0xFF);
            bytes[1] = (byte) ((value >> 8) & 0xFF);

            return bytes;
        }
    }
}
