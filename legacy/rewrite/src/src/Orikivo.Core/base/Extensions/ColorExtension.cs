using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Orikivo
{
    public static class ColorExtension
    {
        public static int FindClosestMatch(this Color col, Color[] palette)
            => BitmapManager.GetClosestMatchingColor(col, palette);

        public static Color[] ByBrightest(this Color[] colors) // to get the brightest color...
            => colors.OrderByDescending(x => x.AsValue()).ToArray();

        public static void ToHSV(this Color col, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(col.R, Math.Max(col.G, col.B));
            int min = Math.Min(col.R, Math.Min(col.G, col.B));

            hue = col.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }

        public static double AsValue(this Color col)
        {
            col.ToHSV(out double hue, out double saturation, out double value);
            return value;
        }
    }
}