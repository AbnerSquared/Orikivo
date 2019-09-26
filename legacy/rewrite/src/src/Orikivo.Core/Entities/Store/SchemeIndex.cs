using System.Collections.Generic;
using System.Drawing;

namespace Orikivo
{
    public class SchemeIndex
    {
        public SchemeIndex() {}

        //public CardColorScheme Baseline = new CardColorScheme("Baseline", 50, ColorScheme.Default);

        private static Color[] _oriGreenPalette =
        {
            Color.FromArgb(110, 250, 200),
            Color.FromArgb(96, 226, 185),
            Color.FromArgb(82, 202, 170),
            Color.FromArgb(68, 178, 155),
            Color.FromArgb(54, 154, 140),
            Color.FromArgb(40, 130, 125),
            Color.FromArgb(26, 106, 110),
            Color.FromArgb(12, 82, 95)
        };

        public OldCardColorScheme OriGreen = new OldCardColorScheme("origreen", 100, _oriGreenPalette);

        private static Color[] _fadingRedPalette = 
        {
            Color.FromArgb(248, 66, 125),
            Color.FromArgb(225, 61, 117),
            Color.FromArgb(203, 57, 109),
            Color.FromArgb(181, 51, 103),
            Color.FromArgb(159, 46, 97),
            Color.FromArgb(136, 41, 89),
            Color.FromArgb(114, 36, 81),
            Color.FromArgb(92, 31, 73)
        };

        private static Color[] _neonRedPalette =
        {
            Color.FromArgb(247, 45, 110),
            Color.FromArgb(222, 39, 101),
            Color.FromArgb(197, 34, 92),
            Color.FromArgb(173, 28, 86),
            Color.FromArgb(148, 22, 79),
            Color.FromArgb(123, 17, 70),
            Color.FromArgb(98, 11, 61),
            Color.FromArgb(73, 6, 52)
        };

        //NeonRed
        public OldCardColorScheme FadingRed = new OldCardColorScheme("fadingred", 200, _fadingRedPalette);

        private static Color[] _polarityWhitePalette =
        {

        };

        //public CardColorScheme PolarityWhite = new CardColorScheme("PolarityWhite", 250, _polarityWhitePalette);

        private static Color[] _wumpuniumPalette =
        {
            Color.FromArgb(139, 164, 243),
            Color.FromArgb(127, 151, 231),
            Color.FromArgb(115, 138, 219),
            Color.FromArgb(103, 125, 207),
            Color.FromArgb(91, 112, 195),
            Color.FromArgb(79, 99, 183),
            Color.FromArgb(67, 86, 171),
            Color.FromArgb(52, 72, 159)
        };

        public OldCardColorScheme Wumpunium = new OldCardColorScheme("wumpunium", 300, _wumpuniumPalette);

        // palettes are from brightest to darkest...
    }
}