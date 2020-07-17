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
        public static readonly GammaPalette NeonRed =    new GammaPalette(0x5C1F49, 0x722451, 0x882959, 0x9F2E61, 0xB53367, 0xCB396D, 0xE13D75, 0xF8427D);
        public static readonly GammaPalette GammaGreen = new GammaPalette(0x0C525F, 0x1A6A6E, 0x28827D, 0x369A8C, 0x44B29B, 0x52CAAA, 0x60E2B9, 0x6EFAC8);

        // NOTE: These are already gradient color maps. They should not be allowed to fuse.
        // REF: https://lospec.com/palette-list/nyx8
        public static readonly GammaPalette Alconia =    new GammaPalette(0x08141E, 0x0F2A3F, 0x20394F, 0x4E495F, 0x816271, 0x997577, 0xC3A38A, 0xF6D6BD);
        public static readonly GammaPalette Glass =      new GammaPalette(0x1C595E, 0x2B6C74, 0x31807F, 0x469996, 0x52A6A1, 0x87D0C8, 0xA8E7DF, 0xDEFFFA);

        /// <summary>
        /// Merges two <see cref="GammaPalette"/> values together, merging the foreground value with the background value by a specified strength.
        /// </summary>
        public static GammaPalette Merge(GammaPalette background, GammaPalette foreground, float strength)
        {
            List<ImmutableColor> colors = new List<ImmutableColor>();

            for (int g = 0; g < RequiredLength; g++)
                colors.Add(ImmutableColor.Merge(background.Values[g], foreground.Values[g], strength));

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
