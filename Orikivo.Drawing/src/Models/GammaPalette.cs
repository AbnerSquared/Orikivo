using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Drawing
{
    public class GammaPalette
    {
        public const int RequiredLength = 8;

        // NOTE: The color values must go from darkest to brightest in order to properly function.
        public static readonly GammaPalette Default =    new GammaPalette(0x000000, 0x242424, 0x494949, 0x6D6D6D, 0x929292, 0xB6B6B6, 0xDBDBDB, 0xFFFFFF);
        public static readonly GammaPalette Polarity =   new GammaPalette(0x677E99, 0x768EA8, 0x849EB8, 0x93ADC4, 0x9FB9CF, 0xB4CCDE, 0xCCE1F0, 0xE3F4FF);
        public static readonly GammaPalette Lemon =      new GammaPalette(0xAF6534, 0xB4793A, 0xBF9442, 0xC9A047, 0xD5B750, 0xE1D05A, 0xEFE465, 0xFBFD77);
        public static readonly GammaPalette Amber =      new GammaPalette(0x9A3E00, 0xA84E08, 0xB85E0F, 0xC46B12, 0xD17A17, 0xD9871C, 0xEB9B23, 0xF8AE2C);
        public static readonly GammaPalette Oceanic =    new GammaPalette(0x273B8F, 0x2E4E99, 0x3660A8, 0x3C70B5, 0x4483C2, 0x4D98D1, 0x57A8DE, 0x65BDEF);
        public static readonly GammaPalette NeonRed =    new GammaPalette(0x5C1F49, 0x722451, 0x882959, 0x9F2E61, 0xB53367, 0xCB396D, 0xE13D75, 0xF8427D);
        public static readonly GammaPalette GammaGreen = new GammaPalette(0x0C525F, 0x1A6A6E, 0x28827D, 0x369A8C, 0x44B29B, 0x52CAAA, 0x60E2B9, 0x6EFAC8);
        public static readonly GammaPalette Wumpite =    new GammaPalette(0x34489F, 0x4356AB, 0x4F63B7, 0x5B70C3, 0x677DCF, 0x738ADB, 0x7F97E7, 0x8BA4F3);
        public static readonly GammaPalette Bubblegum =  new GammaPalette(0xB0608F, 0xB8699D, 0xC274AE, 0xCF84C2, 0xD996D3, 0xE5ACE5, 0xEBBBEF, 0xF8D6FF);

        // NOTE: These are already gradient color maps. They should not be allowed to fuse.
        // REF: https://lospec.com/palette-list/nyx8
        public static readonly GammaPalette Alconia =    new GammaPalette(0x08141E, 0x0F2A3F, 0x20394F, 0x4E495F, 0x816271, 0x997577, 0xC3A38A, 0xF6D6BD);
        public static readonly GammaPalette Glass =      new GammaPalette(0x1C595E, 0x2B6C74, 0x31807F, 0x469996, 0x52A6A1, 0x87D0C8, 0xA8E7DF, 0xDEFFFA);
        public static readonly GammaPalette Chocolate =  new GammaPalette(0x3F1402, 0x5A200D, 0x733520, 0x81422E, 0x8E523E, 0x99614E, 0xA06E5D, 0xAD7B6B);

        /// <summary>
        /// Merges two <see cref="GammaPalette"/> values together, merging the foreground value with the background value by a specified strength.
        /// </summary>
        public static GammaPalette Merge(GammaPalette background, GammaPalette foreground, float strength)
        {
            List<ImmutableColor> colors = new List<ImmutableColor>();

            for (int g = 0; g < RequiredLength; g++)
                colors.Add(ImmutableColor.Blend(background.Values[g], foreground.Values[g], strength));

            return new GammaPalette(colors.ToArray());
        }

        /// <summary>
        /// Smears two <see cref="GammaPalette"/> values together, slowly transitioning the primary palette to the secondary palette.
        /// </summary>
        public static GammaPalette Smear(GammaPalette a, GammaPalette b)
        {
            var colors = new List<ImmutableColor>();

            float mergeStrength = 1.0f / (RequiredLength - 1);

            for (int g = 0; g < RequiredLength; g++)
            {
                var strength = mergeStrength * g;

                //if (g == 0)
                //    strength = 0.1f;

                colors.Add(ImmutableColor.Blend(a[g], b[g], strength));
            }

            return new GammaPalette(colors.ToArray());
        }

        public GammaPalette(params int[] rgbValues)
        {
            if (rgbValues.Length != RequiredLength)
                throw new ArgumentException("A GammaColorMap requires eight unique color values.");

            Values = rgbValues.Select(x => new ImmutableColor((uint)x)).ToArray();
        }

        public GammaPalette(params long[] argbValues)
        {
            if (argbValues.Length != RequiredLength)
                throw new ArgumentException("A GammaColorMap requires eight unique color values.");

            Values = argbValues.Select(x => new ImmutableColor(x)).ToArray();
        }

        public GammaPalette(params ImmutableColor[] colors)
        {
            if (colors == null)
                throw new ArgumentException("A GammaPalette requires an existing list of colors.");

            if (colors.Length != RequiredLength)
                throw new ArgumentException("A GammaPalette requires eight unique color values.");

            Values = colors;
        }

        public ImmutableColor[] Values { get; }

        public ImmutableColor GetValue(Gamma gamma)
            => Values[(int)gamma];

        public ImmutableColor this[Gamma g]
            => GetValue(g);

        public ImmutableColor this[int i]
            => Values[i];
    }
}
