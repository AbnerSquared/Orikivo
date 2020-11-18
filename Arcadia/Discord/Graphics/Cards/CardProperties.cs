using Orikivo.Drawing;
using Orikivo.Text;

namespace Arcadia.Graphics
{
    public class CardProperties
    {
        public static readonly CardProperties Default = new CardProperties
        {
            Trim = false,
            Deny = 0,
            Casing = Casing.Upper,
            Font = FontType.Foxtrot,
            Scale = ImageScale.Medium
        };

        public bool Trim { get; set; }

        /// <summary>
        /// Defines the collection of card groups to exclude from rendering.
        /// </summary>
        public CardGroup Deny { get; set; }

        public Casing Casing { get; set; } = Casing.Upper;

        public FontType Font { get; set; } = FontType.Foxtrot;

        public GammaPalette Palette { get; set; }

        public GammaPalette Outline { get; set; }

        // TODO: Go with a Dictionary<CardGroup, FillInfo> instead?
        // - This is for re-coloring specific components
        // public Dictionary<CardGroup, Gamma?> Gamma { get; set; }

        /// <summary>
        /// Defines the finalized card image scale.
        /// </summary>
        public ImageScale Scale { get; set; } = ImageScale.Medium;
    }
}
