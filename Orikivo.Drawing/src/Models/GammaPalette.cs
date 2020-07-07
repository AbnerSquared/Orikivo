using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Drawing
{
    public class GammaPalette
    {
        public const int RequiredLength = 8;
        
        // NOTE: The color values must go from darkest to brightest in order to properly function.
        public static GammaPalette Default =    new GammaPalette(0x000000, 0x242424, 0x494949, 0x6D6D6D, 0x929292, 0xB6B6B6, 0xDBDBDB, 0xFFFFFF);
        public static GammaPalette NeonRed =    new GammaPalette(0x5C1F49, 0x722451, 0x882959, 0x9F2E61, 0xB53367, 0xCB396D, 0xE13D75, 0xF8427D);
        public static GammaPalette GammaGreen = new GammaPalette(0x0C525F, 0x1A6A6E, 0x28827D, 0x369A8C, 0x44B29B, 0x52CAAA, 0x60E2B9, 0x6EFAC8);

        // NOTE: These are already gradient color maps. They should not be allowed to fuse.
        public static GammaPalette Alconia =    new GammaPalette(0x08141E, 0x0F2A3F, 0x20394F, 0x4E495F, 0x816271, 0x997577, 0xC3A38A, 0xF6D6BD); // Nyx8: https://lospec.com/palette-list/nyx8

        // NOTE: These color maps contain unique properties. (The Alpha component is 0x80 (50%).)
        public static GammaPalette Glass =      new GammaPalette(0x1C595E, 0x2B6C74, 0x31807F, 0x469996, 0x52A6A1, 0x87D0C8, 0xA8E7DF, 0xDEFFFA);

        /// <summary>
        /// Merges two <see cref="GammaPalette"/> values together, merging the foreground value with the background value by a specified strength.
        /// </summary>
        public static GammaPalette Merge(GammaPalette background, GammaPalette foreground, float strength)
        {
            List<GammaColor> colors = new List<GammaColor>();

            for (int g = 0; g < RequiredLength; g++)
                colors.Add(GammaColor.Merge(background.Values[g], foreground.Values[g], strength));

            return new GammaPalette(colors.ToArray());
        }

        public GammaPalette(params int[] rgbValues)
        {
            if (rgbValues.Length != RequiredLength)
                throw new ArgumentException("A GammaColorMap requires eight unique color values.");

            Values = rgbValues.Select(x => new GammaColor((uint)x)).ToArray();
        }

        public GammaPalette(params long[] argbValues)
        {
            if (argbValues.Length != RequiredLength)
                throw new ArgumentException("A GammaColorMap requires eight unique color values.");

            Values = argbValues.Select(x => new GammaColor(x)).ToArray();
        }

        public GammaPalette(params GammaColor[] colors)
        {
            if (colors == null)
                throw new ArgumentException("A GammaPalette requires an existing list of colors.");

            if (colors.Length != RequiredLength)
                throw new ArgumentException("A GammaPalette requires eight unique color values.");

            Values = colors;
        }

        public GammaColor[] Values { get; }
        
        public GammaColor GetValue(Gamma gamma)
            => Values[(int)gamma];

        public GammaColor this[Gamma g]
            => GetValue(g);

        public GammaColor this[int i]
            => Values[i];
    }
}
