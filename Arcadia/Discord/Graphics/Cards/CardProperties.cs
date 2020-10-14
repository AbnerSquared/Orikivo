using Orikivo.Drawing;
using System.Collections.Generic;
using System.Drawing;
using Orikivo;
using Orikivo.Text;

namespace Arcadia.Graphics
{
    public class CardProperties
    {
        public static CardProperties Default = new CardProperties
        {
            Trim = false,
            Deny = CardDeny.None,
            Border = BorderAllow.All,
            Casing = Casing.Upper,
            Font = FontType.Foxtrot,
            Palette = PaletteType.Default,
            Padding = new Padding(2),
            Scale = ImageScale.Medium,
            Gamma = new Dictionary<CardComponent, Gamma?>
            {
                [CardComponent.Activity] = Orikivo.Drawing.Gamma.Max,
                [CardComponent.Avatar] = Orikivo.Drawing.Gamma.Max,
                //[CardComponent.Background] = null,
                //[CardComponent.Border] = Orikivo.Drawing.Gamma.Max,
                [CardComponent.Username] = Orikivo.Drawing.Gamma.Max,
                [CardComponent.Exp] = Orikivo.Drawing.Gamma.Max,
                [CardComponent.Bar] = Orikivo.Drawing.Gamma.Bright,
                [CardComponent.Level] = Orikivo.Drawing.Gamma.Max,
                [CardComponent.Money] = Orikivo.Drawing.Gamma.Max
            }
        };

        public bool Trim { get; set; }

        public CardDeny Deny { get; set; }

        public BorderAllow Border { get; set; } = BorderAllow.All;

        public Casing Casing { get; set; } = Casing.Upper;

        public FontType Font { get; set; } = FontType.Foxtrot;

        public PaletteType Palette { get; set; } = PaletteType.Default;

        public GammaPalette PaletteOverride { get; set; } = null;

        public Color? OutlineColor { get; set; } = null;

        public Dictionary<CardComponent, Gamma?> Gamma { get; set; }

        public Padding Padding { get; set; }

        public ImageScale Scale { get; set; }
    }
}
