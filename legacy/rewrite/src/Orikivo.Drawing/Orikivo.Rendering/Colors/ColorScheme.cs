using Discord;
using System.Drawing;
using SysColor = System.Drawing.Color;
using DiscordColor = Discord.Color;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace Orikivo
{
    /// <summary>
    /// Represents a malleable collection of colors.
    /// </summary>
    public class ColorScheme2
    {
        public List<OriColor> Colors {get; set;}
    }

    /// <summary>
    /// A dynamic color that is malleable to Discord.Color and System.Drawing.Color.
    /// </summary>
    public class OriColor
    {
        public static OriColor Default = new OriColor(129, 243, 193);
        // a collection of preset colors...
        public static OriColor Sunrise = new OriColor(234, 105, 93);

        public static Range ByteLimit = new Range(byte.MinValue, byte.MaxValue);

        //public OriColor(uint rawValue) {}
        public OriColor(int r, int g, int b)
        {
            if (!r.IsInRange(ByteLimit) || !g.IsInRange(ByteLimit) || !b.IsInRange(ByteLimit))
                throw new ArgumentException("The specified integers are out of range.");

            R = (byte)r;
            G = (byte)g;
            B = (byte)b;
            A = (byte)ByteLimit.Max;
        }
        public OriColor(int r, int g, int b, int a)
        {
            if (!r.IsInRange(ByteLimit) || !g.IsInRange(ByteLimit) || !b.IsInRange(ByteLimit) || !a.IsInRange(ByteLimit))
                throw new ArgumentException("The specified integers are out of range.");

            R = (byte)r;
            G = (byte)g;
            B = (byte)b;
            A = (byte)a;
        }

        //public uint RawValue { get; }
        [JsonProperty("r")]
        public byte R { get; }
        [JsonProperty("g")]
        public byte G { get; }
        [JsonProperty("b")]
        public byte B { get; }
        [JsonProperty("a")]
        public byte? A { get; }

        public static implicit operator DiscordColor(OriColor c)
            => new DiscordColor(c.R, c.G, c.B);
        public static explicit operator OriColor(DiscordColor c)
            => new OriColor(c.R, c.G, c.B);

        public static implicit operator SysColor(OriColor c)
            => SysColor.FromArgb(c.A ?? 255, c.R, c.G, c.B);
        public static explicit operator OriColor(SysColor c)
            => new OriColor(c.R, c.G, c.B, c.A);
    }
}