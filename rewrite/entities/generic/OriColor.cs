using SysColor = System.Drawing.Color;
using DiscordColor = Discord.Color;
using Newtonsoft.Json;

namespace Orikivo
{
    // based off of System.Color
    // the color class used to allow bridging between System.Color and Discord.Color
    // OriColor.FromHex(), OriColor.FromColor(), OriColor.OriGreen
    public class OriColor
    {
        public static OriColor OriGreen = new OriColor(0x6EFAC8);
        private const int _aShift = 24;
        private const int _rShift = 16;
        private const int _gShift = 8;
        private const int _bShift = 0;

        [JsonConstructor]
        internal OriColor(long rValue)
        {
            Value = rValue;
        }

        public OriColor(uint rgb)
        {
            Value = MakeArgb(rgb, 255);
        }

        public OriColor(byte r, byte g, byte b)
        {
            Value = MakeArgb(r, g, b, 255);
        }

        public OriColor(byte r, byte g, byte b, byte a)
        {
            Value = MakeArgb(r, g, b, a);
        }

        [JsonProperty("rvalue")]
        public long Value { get; }

        [JsonIgnore]
        public byte A => (byte)((Value >> _aShift));
        [JsonIgnore]
        public byte R => (byte)((Value >> _rShift));
        [JsonIgnore]
        public byte G => (byte)((Value >> _gShift));
        [JsonIgnore]
        public byte B => (byte)((Value >> _bShift));

        private static long MakeArgb(uint rgb, byte a)
            => MakeArgb((byte)(rgb >> _rShift), (byte)(rgb >> _gShift), (byte)(rgb >> _bShift), a);
        private static long MakeArgb(byte r, byte g, byte b, byte a)
            => (long)(unchecked((uint)(r << _rShift |
                g << _gShift |
                b << _bShift |
                a << _aShift))) & 0xffffffff;

        private static uint MakeRgb(byte r, byte g, byte b)
            => (unchecked((uint)(r << _rShift | g << _gShift | b << _bShift))) & 0xffffff;

        // discord ignores alpha integer
        public static implicit operator DiscordColor(OriColor c)
            => new DiscordColor((uint)c.Value << 8 >> 8);
        public static explicit operator OriColor(DiscordColor c)
            => new OriColor(c.RawValue);

        public static implicit operator SysColor(OriColor c)
            => SysColor.FromArgb((int)c.Value);
        public static explicit operator OriColor(SysColor c)
            => new OriColor((uint)c.ToArgb());

        public override string ToString()
            => A < 255 ? string.Format("#{0:X8}", Value) : string.Format("#{0:X6}", MakeRgb(R, G, B));
    }
}
