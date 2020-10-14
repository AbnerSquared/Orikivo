using Orikivo.Drawing;
using System.Collections.Generic;
using Orikivo.Text;

namespace Orikivo.Canary
{
    // NOTE: This service will most likely be moved to Arcadia, as Orikivo only focuses on the RPG graphics handler.
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
            Gamma = new Dictionary<CardComponentType, Gamma?>
            {
                [CardComponentType.Activity] = Drawing.Gamma.Max,
                [CardComponentType.Avatar] = Drawing.Gamma.Max,
                [CardComponentType.Background] = null,
                [CardComponentType.Border] = Drawing.Gamma.Max,
                [CardComponentType.Username] = Drawing.Gamma.Max
            }
        };

        public bool Trim { get; set; } = false;

        public CardDeny Deny { get; set; }

        public BorderAllow Border { get; set; } = BorderAllow.All;

        public Casing Casing { get; set; } = Casing.Upper;

        public FontType Font { get; set; } = FontType.Foxtrot;

        public PaletteType Palette { get; set; } = PaletteType.Default;

        public Dictionary<CardComponentType, Gamma?> Gamma { get; set; }

        public Padding Padding { get; set; }

        public ImageScale Scale { get; set; }
    }
}
