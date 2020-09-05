using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Orikivo.Drawing
{
    /// <summary>
    /// Represents an immutable color object that supports multi-tone grouping and conversion formulas.
    /// </summary>
    public readonly struct ImmutableColor
    {
        public static readonly ImmutableColor Default = new ImmutableColor(0xFFFFFF);
        public static readonly ImmutableColor NeonRed = new ImmutableColor(0xF8427D);
        public static readonly ImmutableColor GammaGreen = new ImmutableColor(0x6EFAC8);

        private const int A_SHIFT = 24;
        private const int R_SHIFT = 16;
        private const int G_SHIFT = 8;
        private const int B_SHIFT = 0;

        private const float R_LUMINANCE = 0.2126f;
        private const float G_LUMINANCE = 0.7152f;
        private const float B_LUMINANCE = 0.0722f;

        private static byte GetByteAmount(float amount)
        {
            if (!RangeF.Percent.Contains(amount))
                throw new ArithmeticException("The amount value specified is outside of the range [0.0, 1.0].");

            return (byte) MathF.Floor(255 * amount);
        }

        public static ImmutableColor FromRange(float r, float g, float b, float a = 1.0f)
        {
            return new ImmutableColor(GetByteAmount(r),
                                  GetByteAmount(g),
                                  GetByteAmount(b),
                                  GetByteAmount(a));
        }

        public static ImmutableColor FromCmyk(float c, float m, float y, float k)
        {
            RangeF bounds = RangeF.Percent;

            if (!bounds.All(c, m, y, k))
                throw new ArgumentException("One of the specified float values are out of range.");

            byte r = GetCmy(c, k);
            byte g = GetCmy(m, k);
            byte b = GetCmy(y, k);

            return new ImmutableColor(r, g, b);
        }

        private static byte GetCmy(float v, float k)
        {
            return (byte) Math.Floor(255 * (1 - v) * (1 - k));
        }

        // TODO: Optimize this method
        public static ImmutableColor FromHex(string hex)
        {
            hex = hex.TrimStart('#');

            string format = "0x{0}";

            byte r = byte.Parse(string.Format(format, hex[0] + hex[1]));
            byte g = byte.Parse(string.Format(format, hex[2] + hex[3]));
            byte b = byte.Parse(string.Format(format, hex[4] + hex[5]));

            return new ImmutableColor(r, g, b);
        }

        public static ImmutableColor FromHsl(AngleF hue, float s, float l)
        {
            if (!RangeF.Percent.All(s, l))
                throw new ArgumentException("One of the specified float values are out of range.");

            float c = (1 - Math.Abs((2 * l) - 1)) * s;
            float x = c * (1 - Math.Abs((hue.Degrees / 60.00f) % 2 - 1));
            float m = l - (c / 2);

            return CreateFromChmx(c, hue.Degrees, m, x);
        }

        public static ImmutableColor FromHsv(AngleF h, float s, float v)
        {
            if (!RangeF.Percent.All(s, v))
                throw new ArgumentException("One of the specified float values are out of range.");

            float c = v * s;
            float x = c * (1 - Math.Abs((h.Degrees / 60.00f) % 2 - 1));
            float m = v - c;

            return CreateFromChmx(c, h.Degrees, m, x);

        }

        private static ImmutableColor CreateFromChmx(float c, float h, float m, float x)
        {
            float rF = 0.00f;
            float gF = 0.00f;
            float bF = 0.00f;

            if (RangeF.Contains(0.00f, 60.00f, h, true, false))
            {
                rF = c;
                gF = x;
                bF = 0;
            }
            else if (RangeF.Contains(60.00f, 120.00f, h, true, false))
            {
                rF = x;
                gF = c;
                bF = 0;
            }
            else if (RangeF.Contains(120.00f, 180.00f, h, true, false))
            {
                rF = 0;
                gF = c;
                bF = x;
            }
            else if (RangeF.Contains(180.00f, 240.00f, h, true, false))
            {
                rF = 0;
                gF = x;
                bF = c;
            }
            else if (RangeF.Contains(240.00f, 300.00f, h, true, false))
            {
                rF = x;
                gF = 0;
                bF = c;
            }
            else if (RangeF.Contains(300.00f, 360.00f, h, true, false))
            {
                rF = c;
                gF = 0;
                bF = x;
            }

            byte r = (byte)Math.Floor((rF + m) * 255);
            byte g = (byte)Math.Floor((gF + m) * 255);
            byte b = (byte)Math.Floor((bF + m) * 255);

            return new ImmutableColor(r, g, b);
        }

        /// <summary>
        /// Returns a <see cref="ImmutableColor"/> that is the conversion of the specified <see cref="ImmutableColor"/> to grayscale.
        /// </summary>
        public static ImmutableColor Grayscale(ImmutableColor value)
        {
            byte rgb = (byte) Math.Floor(R_LUMINANCE * value.R + G_LUMINANCE * value.G + B_LUMINANCE * value.B);
            return new ImmutableColor(rgb, rgb, rgb);
        }

        public static float GetBrightness(ImmutableColor value)
            => MathF.Floor(R_LUMINANCE * value.R + G_LUMINANCE * value.G + B_LUMINANCE * value.B);

        // Keep the opacity of the initial alpha
        public static ImmutableColor Blend(ImmutableColor background, ImmutableColor foreground, float strength)
        {
            // TODO: Ensure range of 0.0 to 1.0
            if (strength > 1.00f || strength < 0.00f)
                throw new Exception("The specified merge strength must be within the range of 0.00f to 1.00f.");

            byte r = GetBlendValue(background.R, foreground.R, strength);
            byte g = GetBlendValue(background.G, foreground.G, strength);
            byte b = GetBlendValue(background.B, foreground.B, strength);

            return new ImmutableColor(r, g, b);
        }

        private static byte GetBlendValue(byte b, byte f, float strength)
            => (byte)Math.Floor((b * (1.00f - strength)) + (f * strength));

        /// <summary>
        /// Gets the distance between two <see cref="ImmutableColor"/> values.
        /// </summary>
        public static int Distance(ImmutableColor from, ImmutableColor to)
        {
            int r = to.R - from.R;
            int g = to.G - from.G;
            int b = to.B - from.B;
            return r * r + g * g + b * b;
        }

        /// <summary>
        /// Returns the average <see cref="ImmutableColor"/> between two <see cref="ImmutableColor"/> values.
        /// </summary>
        public static ImmutableColor Average(ImmutableColor a, ImmutableColor b)
        {
            byte red = GetAverage(a.R, b.R);
            byte green = GetAverage(a.G, b.G);
            byte blue = GetAverage(a.B, b.B);

            return new ImmutableColor(red, green, blue);
        }

        /// <summary>
        /// Returns the average <see cref="ImmutableColor"/> from an array of <see cref="ImmutableColor"/> values.
        /// </summary>
        public static ImmutableColor Average(ImmutableColor[] colors)
        {
            byte r = GetAverage(colors.Select(x => x.R).Cast<int>());
            byte g = GetAverage(colors.Select(x => x.G).Cast<int>());
            byte b = GetAverage(colors.Select(x => x.B).Cast<int>());

            return new ImmutableColor(r, g, b);
        }

        private static byte GetAverage(in IEnumerable<int> values)
        {
            if (!values.Any())
                throw new ArgumentException("There must at least be one specified value in order to get an average.");

            if (values.Count() == 1)
                return (byte)values.ElementAt(0);

            return (byte)Math.Floor(values.Sum() / (double)values.Count());
        }

        // TODO: Create GetAverage method for all integer types.
        private static byte GetAverage(params int[] values)
            => GetAverage((IEnumerable<int>) values);

        /// <summary>
        /// Returns a <see cref="ImmutableColor"/> from the difference between two <see cref="ImmutableColor"/> values.
        /// </summary>
        public static ImmutableColor Difference(ImmutableColor a, ImmutableColor b)
        {
            byte red = (byte) Math.Abs(a.R - b.R);
            byte green = (byte) Math.Abs(a.G - b.G);
            byte blue = (byte) Math.Abs(a.B - b.B);

            return new ImmutableColor(red, green, blue);
        }

        public static ImmutableColor Addition(ImmutableColor a, ImmutableColor b)
        {
            byte red = (byte) Math.Min(255, a.R + b.R);
            byte green = (byte) Math.Min(255, a.G + b.G);
            byte blue = (byte) Math.Min(255, a.B + b.B);

            return new ImmutableColor(red, green, blue);
        }

        public static ImmutableColor Subtract(ImmutableColor a, ImmutableColor b)
        {
            byte red = (byte) Math.Min(0, a.R - b.R);;
            byte green = (byte) Math.Min(0, a.G - b.G);
            byte blue = (byte) Math.Min(0, a.B - b.B);

            return new ImmutableColor(red, green, blue);
        }

        public static ImmutableColor DarkenOnly(ImmutableColor a, ImmutableColor b)
        {
            byte red = Math.Min(a.R, b.R);
            byte green = Math.Min(a.G, b.G);
            byte blue = Math.Min(a.B, b.G);

            return new ImmutableColor(red, green, blue);
        }

        public static ImmutableColor LightenOnly(ImmutableColor a, ImmutableColor b)
        {
            byte red = Math.Max(a.R, b.R);
            byte green = Math.Max(a.G, b.G);
            byte blue = Math.Max(a.B, b.B);

            return new ImmutableColor(red, green, blue);
        }

        /// <summary>
        /// Returns a <see cref="ImmutableColor"/> that is the opposite of the specified <see cref="ImmutableColor"/>.
        /// </summary>
        public static ImmutableColor Negative(ImmutableColor value)
        {
            byte r = (byte) (255 - value.R);
            byte g = (byte) (255 - value.G);
            byte b = (byte) (255 - value.B);

            return new ImmutableColor(r, g, b);
        }

        /// <summary>
        /// Returns the closest matching <see cref="ImmutableColor"/> for the specified value from an array of <see cref="ImmutableColor"/> values.
        /// </summary>
        public static ImmutableColor ClosestMatch(ImmutableColor value, ImmutableColor[] colors)
        {
            return colors[ClosestMatchAt(value, colors)];
        }

        /// <summary>
        /// Returns the index of the closest matching index of the specified <see cref="ImmutableColor"/> from an array of <see cref="ImmutableColor"/> values.
        /// </summary>
        public static int ClosestMatchAt(ImmutableColor value, ImmutableColor[] colors)
        {
            int leastIndex = 0;
            int least = int.MaxValue;

            for (int i = 0; i < colors.Length; i++)
            {
                int dist = Distance(value, colors[i]);

                if (dist >= least)
                    continue;

                leastIndex = i;
                least = dist;

                if (dist == 0)
                    return i;
            }

            return leastIndex;
        }

        /// <summary>
        /// Returns the closest matching <see cref="ImmutableColor"/> for the specified value from an array of <see cref="ImmutableColor"/> values.
        /// </summary>
        public static ImmutableColor ClosestLumen(ImmutableColor value, ImmutableColor[] colors)
        {
            return colors[ClosestBrightnessAt(value, colors)];
        }

        public static int ClosestBrightnessAt(ImmutableColor value, ImmutableColor[] colors)
        {
            int index = (int) Math.Floor(GetBrightness(value) / (255 / (double) colors.Length));

            if (index == colors.Length)
                return colors.Length - 1;

            return index;
        }

        public static ImmutableColor GetDarkestColor(ImmutableColor[] values)
        {
            float darkest = 0;
            int index = 0;

            for (int i = 0; i < values.Length; i++)
            {
                float brightness = GetBrightness(values[i]);

                if (brightness >= darkest)
                    continue;

                index = i;
                darkest = brightness;
            }

            return values[index];
        }

        public static ImmutableColor GetBrightestColor(ImmutableColor[] values)
        {
            float brightest = 0;
            int index = 0;

            for (int i = 0; i < values.Length; i++)
            {
                float brightness = GetBrightness(values[i]);

                if (brightness <= brightest)
                    continue;

                index = i;
                brightest = brightness;
            }

            return values[index];
        }

        /// <summary>
        /// Returns the index of the closest matching index of the specified <see cref="ImmutableColor"/> from a <see cref="GammaPalette"/>.
        /// </summary>
        public static int ClosestMatchAt(ImmutableColor value, GammaPalette colors)
            => ClosestMatchAt(value, colors.Values);

        /// <summary>
        /// Returns the closest matching <see cref="ImmutableColor"/> for the specified value from a <see cref="GammaPalette"/>.
        /// </summary>
        public static ImmutableColor ClosestMatch(ImmutableColor value, GammaPalette colors)
            => ClosestMatch(value, colors.Values);

        /// <summary>
        /// Constructs a new <see cref="ImmutableColor"/> from a raw RGBA value.
        /// </summary>
        [JsonConstructor]
        internal ImmutableColor(long rgba)
        {
            Value = rgba;
        }

        /// <summary>
        /// Constructs a new <see cref="ImmutableColor"/> from a raw RGB value.
        /// </summary>
        public ImmutableColor(uint rgb)
        {
            Value = MakeArgb(rgb, 255);
        }

        /// <summary>
        /// Constructs a new <see cref="ImmutableColor"/> from the specified R, G, and B values.
        /// </summary>
        public ImmutableColor(byte r, byte g, byte b)
        {
            Value = MakeArgb(r, g, b, 255);
        }

        /// <summary>
        /// Constructs a new <see cref="ImmutableColor"/> from the specified R, G, B, and A values.
        /// </summary>
        public ImmutableColor(byte r, byte g, byte b, byte a)
        {
            Value = MakeArgb(r, g, b, a);
        }

        /// <summary>
        /// Gets the raw value of this <see cref="ImmutableColor"/>.
        /// </summary>
        [JsonProperty("raw_value")]
        public long Value { get; }

        /// <summary>
        /// Gets the Alpha component of this <see cref="ImmutableColor"/>.
        /// </summary>
        [JsonIgnore]
        public byte A => (byte)(Value >> A_SHIFT);

        /// <summary>
        /// Gets the Red component of this <see cref="ImmutableColor"/>.
        /// </summary>
        [JsonIgnore]
        public byte R => (byte)(Value >> R_SHIFT);

        /// <summary>
        /// Gets the Green component of this <see cref="ImmutableColor"/>.
        /// </summary>
        [JsonIgnore]
        public byte G => (byte)(Value >> G_SHIFT);

        /// <summary>
        /// Gets the Blue component of this <see cref="ImmutableColor"/>.
        /// </summary>
        [JsonIgnore]
        public byte B => (byte)(Value >> B_SHIFT);

        private static long MakeArgb(uint rgb, byte a)
            => MakeArgb((byte)(rgb >> R_SHIFT), (byte)(rgb >> G_SHIFT), (byte)(rgb >> B_SHIFT), a);

        private static long MakeArgb(byte r, byte g, byte b, byte a)
            => (long) unchecked((uint)(r << R_SHIFT | g << G_SHIFT | b << B_SHIFT | a << A_SHIFT)) & 0xffffffff;

        private static uint MakeRgb(byte r, byte g, byte b)
            => unchecked((uint)(r << R_SHIFT | g << G_SHIFT | b << B_SHIFT)) & 0xffffff;

        /// <summary>
        /// Returns a string that represents the hexadecimal format of this <see cref="ImmutableColor"/>.
        /// </summary>
        public override string ToString()
            => A < 255 ? string.Format("#{0:X8}", Value) : string.Format("#{0:X6}", MakeRgb(R, G, B));

        public static implicit operator Color(ImmutableColor c)
            => Color.FromArgb((int)c.Value);

        public static implicit operator ImmutableColor(Color c)
            => new ImmutableColor((uint)c.ToArgb());
    }
}
