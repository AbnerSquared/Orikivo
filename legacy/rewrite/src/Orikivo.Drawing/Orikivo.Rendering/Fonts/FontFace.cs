using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    /// <summary>
    /// Represents a font face made for use with pixel renders.
    /// </summary>
    public class FontFace
    {
        public FontFace()
        {

        }

        [JsonConstructor]
        public FontFace(ulong id, string name, FontUnit size, int padding, int spacing, int overhang, List<FontStyle> styles)
        {
            Id = id;
            Name = name;
            Ppu = size;
            Padding = padding;
            Spacing = spacing;
            Overhang = overhang;
            Styles = styles;
        }

        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } // the name of the font. (folder)

        [JsonProperty("ppu")]
        public FontUnit Ppu { get; set; } // font size.

        [JsonProperty("fallback")]
        public bool? HasFallback { get; set; } // if the char has a fallback font.

        [JsonProperty("crop")]
        public bool? AutoCrop { get; set; } // if the font should be automatically cropping letters.

        [JsonProperty("padding")]
        public int Padding { get; set; } // how far a letter shifts right after a previous.

        [JsonProperty("spacing")]
        public int Spacing { get; set; } // how wide spaces should be on the font face.

        [JsonProperty("overhang")]
        public int Overhang { get; set; } // How far a letter is shifted down if overhang is true.

        [JsonProperty("styles")]
        public List<FontStyle> Styles { get; set; } // the list of sheets this font has.

        /// <summary>
        /// Searches for a specified FontSheet from a FontFace.
        /// </summary>
        /// <param name="index">Returns the index of the sheet and the x and y position of the letter.</param>
        public FontSheet Search(char c, FontType type, out (int i, int x, int y) index)
            => Search(GetStyle(type), c, out index);

        /// <summary>
        /// Searches for a specified FontSheet from a FontFace.
        /// </summary>
        /// <param name="index">Returns the index of the sheet and the x and y position of the letter.</param>
        public FontSheet Search(FontStyle style, char c, out (int i, int x, int y) index)
        {
            index = FontManager.FontMap.GetMapIndex(c);
            style.TryGetSheet(index.i, out FontSheet sheet);
            return sheet;
        }

        public FontStyle GetStyle(FontType type)
            => Styles.Where(x => x.Type == type).First();

        public FontStyle GetDefault()
            => GetStyle(FontType.Default);

        public override string ToString()
            => $"FontFace\n{Name} ({Id})\nSize:{Ppu}\nPadding: {Padding}px\nSpace: {Spacing}px\nOverhang: {Overhang}px\nStyles: {Styles.Count}";
    }
}