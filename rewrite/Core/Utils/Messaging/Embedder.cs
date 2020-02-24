using Orikivo.Drawing;
using System;
using System.Collections.Generic;

namespace Orikivo
{
    // TODO: Implement StringResource
    /// <summary>
    /// Utilized with MessagePacket, this handles creating custom embed formats.
    /// </summary>
    public class Embedder
    {
        public static Embedder Default
            => new Embedder { Color = GammaColor.GammaGreen };

        public TextLocale Locale { get; set; } = TextLocale.English;
        public GammaColor? Color { get; set; } // GammaColor
        public string Footer { get; set; }
        public string Author { get; set; }
        public string Header { get; set; }
        public string FooterIconUrl { get; set; }
        public string AuthorIconUrl { get; set; }
        public string HeaderUrl { get; set; }
        public DateTime? Timestamp { get; set; }

        // TODO: Maybe
        public List<Discord.EmbedFieldBuilder> Fields { get; set; }
    }
}
