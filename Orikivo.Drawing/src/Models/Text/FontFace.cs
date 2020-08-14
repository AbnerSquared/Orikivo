using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Orikivo.Drawing
{
    /// <summary>
    /// Represents a font that specifies how characters are drawn.
    /// </summary>
    public class FontFace
    {
        private static FontTag GetTagValue(bool isMonospace, bool isUnicodeSupported)
        {
            FontTag tag = 0;

            if (isMonospace)
                tag |= FontTag.Monospace;

            if (isUnicodeSupported)
                tag |= FontTag.UnicodeSupported;

            return tag;
        }

        public FontFace(FontFaceBuilder builder)
        {
            CharWidth = builder.Width;
            CharHeight = builder.Height;
            Padding = builder.Padding;
            Tag = GetTagValue(builder.IsMonospace, builder.IsUnicodeSupported);
            SheetUrls = builder.SheetUrls;
            Whitespace = builder.Whitespace;
            Overrides = builder.Customs;
            HideBadUnicode = builder.HideBadUnicode;
        }

        [JsonConstructor]
        internal FontFace(int width, int height, FontTag tag, Dictionary<int, string> sheetUrls,
            Padding? padding = null, List<WhiteSpaceInfo> empties = null, List<CharOverride> customs = null,
            bool hideBadUnicode = false)
        {
            CharWidth = width;
            CharHeight = height;
            Padding = padding ?? Padding.Char;
            Tag = tag;
            SheetUrls = sheetUrls;
            Whitespace = empties;
            Overrides = customs;
            HideBadUnicode = hideBadUnicode;
        }

        [JsonProperty("width")]
        public int CharWidth { get; }

        [JsonProperty("height")]
        public int CharHeight { get; }

        [JsonProperty("padding")]
        public Padding Padding { get; }

        [JsonProperty("tag")]
        public FontTag Tag { get; }

        [JsonProperty("sheets")]
        public /*IReadOnly*/Dictionary<int, string> SheetUrls { get; }

        [JsonProperty("empties")]
        public /*IReadOnly*/List<WhiteSpaceInfo> Whitespace { get; }

        // TODO: rename all assets to use overrides instead
        [JsonProperty("customs")]
        public /*IReadOnly*/List<CharOverride> Overrides { get; }

        [JsonProperty("hide_bad_unicode")]
        public bool HideBadUnicode { get; }

        [JsonIgnore]
        public bool IsUnicodeSupported => Tag.HasFlag(FontTag.UnicodeSupported);

        [JsonIgnore]
        public bool IsMonospace => Tag.HasFlag(FontTag.Monospace);

        public Point GetCharOffset(char c)
            => Overrides?.FirstOrDefault(x => x.Chars.Contains(c))?.GetOffset() ?? Point.Empty;

        public int GetCharWidth(char c)
            => GetWhiteSpace(c)?.Width ?? Overrides?.FirstOrDefault(x => x.Chars.Contains(c))?.Width ?? CharWidth;

        public WhiteSpaceInfo GetWhiteSpace(char c)
            => Whitespace.FirstOrDefault(x => x.Chars.Contains(c));
    }
}
