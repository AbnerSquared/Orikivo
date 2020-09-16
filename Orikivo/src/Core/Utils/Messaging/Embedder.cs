using Orikivo.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Orikivo
{
    /// <summary>
    /// Represents a customizable message container.
    /// </summary>
    public class Embedder
    {
        public static Embedder Default
            => new Embedder { Color = ImmutableColor.GammaGreen };

        public TextLocale Locale { get; set; } = TextLocale.English;

        public Color? Color { get; set; }

        public string Footer { get; set; }

        public string Author { get; set; }

        public string Header { get; set; }

        public string FooterIconUrl { get; set; }

        public string AuthorIconUrl { get; set; }

        public string HeaderUrl { get; set; }

        public DateTime? Timestamp { get; set; }

        public List<string> Sections { get; set; }
    }
}
