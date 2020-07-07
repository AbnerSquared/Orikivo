using System.Collections.Generic;

namespace Orikivo.Drawing
{
    /// <summary>
    /// A constructor for <see cref="FontFace"/> used to create custom fonts.
    /// </summary>
    public class FontFaceBuilder
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public Padding Padding { get; set; } = Padding.Char;
        public Dictionary<int, string> SheetUrls { get; set; }
        public List<WhiteSpaceInfo> Whitespace { get; set; }
        public List<CharOverride> Customs { get; set; }
        public bool IsUnicodeSupported { get; set; }
        public bool IsMonospace { get; set; }
        public bool HideBadUnicode { get; set; }
    }
}
