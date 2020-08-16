using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Orikivo.Drawing
{
    /// <summary>
    /// Represents a factory that handles drawing text with a <see cref="FontFace"/>.
    /// </summary>
    public class TextFactory : IDisposable
    {
        public const string DefaultFontDirectory = "../assets/fonts/";

        private readonly bool _cacheable;
        private readonly List<FontFace> _fonts;

        public TextFactory(TextFactoryConfig config = null)
        {
            config ??= TextFactoryConfig.Default;
            _cacheable = config.UseCache;
            CharMap = config.CharMap;
            _fonts = config.Fonts ?? new List<FontFace>();

            if (config.UseCache)
                Cache = new Dictionary<char, CachedChar>();
        }

        public TextFactory(char[][][][] charMap, bool useCache = true)
        {
            _cacheable = useCache;
            CharMap = charMap;
            _fonts = new List<FontFace>();
            Cache = new Dictionary<char, CachedChar>();
        }

        private char[][][][] CharMap { get; }

        private Dictionary<char, CachedChar> Cache { get; }

        public IReadOnlyList<FontFace> Fonts => _fonts;

        public FontFace CurrentFont => Fonts[CurrentFontIndex];

        public bool Disposed { get; private set; }

        private int CurrentFontIndex { get; set; }

        /// <summary>
        /// Imports the specified <see cref="FontFace"/> into this <see cref="TextFactory"/> if it hasn't already been imported.
        /// </summary>
        public void ImportFont(FontFace font)
        {
            if (font is null)
                return;

            if (_fonts.Count > 0)
                if (CurrentFont.Equals(font))
                    return;

            if (!_fonts.Contains(font))
                _fonts.Add(font);
        }

        public void SetFont(FontFace font)
        {
            if (_fonts.Count > 0)
                if (CurrentFont.Equals(font))
                    return;

            ImportFont(font);
            CurrentFontIndex = _fonts.IndexOf(font);
        }

        /// <summary>
        /// Returns the raw sprite from the specified character.
        /// </summary>
        internal Bitmap GetRawChar(char c, FontFace font = null)
        {
            ImportFont(font);
            font ??= CurrentFont;

            if (WhiteSpaceInfo.IsWhiteSpace(c) || c == '\n')
                return null;

            CharIndex i = ImageHelper.GetCharIndex(c, CharMap);

            if (!i.IsSuccess || !font.SheetUrls.Keys.Contains(i.Page))
            {
                // TODO: Return UNKNOWN unicode character
                return null;
            }

            // TODO: Handle directory assignment
            using var bmp = new Bitmap($"{DefaultFontDirectory}{font.SheetUrls[i.Page]}");

            int x = font.CharWidth * i.Column;
            int y = font.CharHeight * i.Row;

            var crop = new Rectangle(x, y, font.CharWidth, font.CharHeight);
            Bitmap tmp = ImageHelper.Crop(bmp, crop);

            return tmp;
        }

        /// <summary>
        /// Returns the sprite from the specified character.
        /// </summary>
        public Bitmap GetChar(char value, FontFace font = null, bool trimEmptyPixels = true)
        {
            ImportFont(font);
            font ??= CurrentFont;

            if (Cache.ContainsKey(value) && _fonts.IndexOf(font) == Cache[value].FontIndex)
                return Cache[value].Value;

            Bitmap bmp = GetRawChar(value, font);

            if (bmp == null)
                return null;

            Cache[value] = new CachedChar(_fonts.IndexOf(font), bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), bmp.PixelFormat));

            if (!font.IsMonospace && trimEmptyPixels)
                return ImageHelper.Crop(bmp, 0, 0, ImageHelper.GetNonEmptyWidth(bmp), font.CharHeight);

            return bmp;
        }

        private static bool IsNonEmptyChar(char c)
            => !WhiteSpaceInfo.IsWhiteSpace(c) && c != '\n';

        /// <summary>
        /// Returns a collection of all unique character sprites to the specified string.
        /// </summary>
        public Dictionary<char, Bitmap> GetChars(string content, FontFace font = null, bool trimEmptyPixels = true)
        {
            ImportFont(font);
            font ??= CurrentFont;

            if (string.IsNullOrWhiteSpace(content))
                return null;

            var charMap = new Dictionary<char, Bitmap>();

            foreach (char t in content.Where(IsNonEmptyChar))
            {
                if (charMap.ContainsKey(t))
                    continue;

                charMap[t] = GetChar(t, font, trimEmptyPixels);
            }

            return charMap;
        }

        // TODO: Implement max width and max height comparisons
        private TextData CreateText(string content, FontFace font, Padding? imagePadding = null, bool trimEmptyPixels = true, bool extendOnOffset = false)
        {
            ImportFont(font);
            font ??= CurrentFont;

            bool canTrim = trimEmptyPixels && !font.IsMonospace;
            Padding padding = imagePadding ?? Padding.Empty;

            if (!extendOnOffset)
                extendOnOffset = font.Overrides?.Any(x => x.GetOffset() != null) ?? false;

            var cursor = new Cursor();
            var charData = new List<CharData>();
            var imageMap = new Dictionary<char, Bitmap>();
            int cursorHeight = font.Padding.Height + font.CharHeight;

            int charIndex = 0;
            int renderWidth = 0;
            int renderHeight = 0;
            int yMaxOffset = 0;

            foreach (char c in content)
            {
                // NOTE: Calculate the character's initial width and padding
                Padding charPadding = font.Padding.Clone();
                int drawWidth = font.GetCharWidth(c);

                // NOTE: Handle all line breaks here
                if (c == '\n')
                {
                    int vX = padding.Left + cursor.X;
                    int vY = padding.Top + cursor.Y;

                    charData.Add(new CharData(null, c, vX, vY, 0, font.CharHeight));

                    if (cursor.X > renderWidth)
                        renderWidth = cursor.X;

                    cursor.X = charPadding.Left;
                    cursor.Y += cursorHeight + 1;

                    if (cursor.Y > renderHeight)
                        renderHeight = cursor.Y;

                    charIndex++;
                    continue;
                }

                // NOTE: Handle all whitespace characters
                if (WhiteSpaceInfo.IsWhiteSpace(c))
                {
                    int emptyWidth = canTrim ? drawWidth : drawWidth + charPadding.Width;
                    int vX = padding.Left + cursor.X;
                    int vY = padding.Top + cursor.Y;

                    charData.Add(new CharData(null, c, vX, vY, emptyWidth, font.CharHeight));
                    cursor.X += emptyWidth;
                    charIndex++;
                    continue;
                }

                // NOTE: Try to assign a matching sprite for this character
                if (!imageMap.ContainsKey(c))
                    imageMap[c] = GetChar(c, font, trimEmptyPixels);

                // NOTE: Ignore handling this sprite if there isn't an image assigned to it
                if (imageMap[c] == null)
                    continue;

                // NOTE: Otherwise, if an image for this character does exist, try to assign the actual width to it

                if (canTrim)
                    drawWidth = imageMap[c].Width;

                int cursorWidth = charPadding.Width + drawWidth;

                yMaxOffset += Math.Max(font.GetCharOffset(c).Y, 0);

                // NOTE: No left padding if it's the first character
                if (charIndex == 0)
                {
                    cursorWidth -= charPadding.Left;
                    charPadding.Left = 0;
                }

                // NOTE: No right padding if it's the final character
                if (charIndex == content.Length - 1)
                {
                    cursorWidth -= charPadding.Right;
                    charPadding.Right = 0;
                }

                if (charIndex > 0 && charIndex < content.Length - 1)
                {
                    char? beforeChar = content.ElementAtOrDefault(charIndex - 1);
                    char? afterChar = content.ElementAtOrDefault(charIndex + 1);

                    if (beforeChar.HasValue && beforeChar != '\n')
                    {
                        cursorWidth -= charPadding.Left;
                        charPadding.Left = 0;
                    }

                    if (afterChar.HasValue)
                    {
                        if (afterChar == '\n' || WhiteSpaceInfo.IsWhiteSpace(afterChar.Value))
                        {
                            cursorWidth -= charPadding.Right;
                            charPadding.Right = 0;
                        }
                    }
                }

                int pX = padding.Left + cursor.X;
                int pY = padding.Top + cursor.Y;
                charData.Add(new CharData(imageMap[c], c, pX, pY, drawWidth, font.CharHeight, charPadding, font.GetCharOffset(c)));
                cursor.X += cursorWidth;
                charIndex++;
            }

            int height = renderHeight + font.CharHeight;
            int width = cursor.X > renderWidth ? cursor.X : renderWidth;

            // NOTE: If the image is extended from offsets, include it to the final height
            if (extendOnOffset)
                height += yMaxOffset;

            return new TextData(content, padding, width, height, charData);
        }

        public Bitmap DrawText(string content, ImageProperties properties = null)
            => DrawText(content, CurrentFont, GammaPalette.Default[Gamma.Max], properties);

        public Bitmap DrawText(string content, FontFace font, ImageProperties properties = null)
            => DrawText(content, font, GammaPalette.Default[Gamma.Max], properties);

        public Bitmap DrawText(string content, Color color, ImageProperties properties = null)
            => DrawText(content, CurrentFont, color, properties);

        public Bitmap DrawText(string content, FontFace font, Color color, ImageProperties properties = null)
        {
            properties ??= new ImageProperties();
            TextData textData = CreateText(content, font, properties.Padding, properties.TrimEmptyPixels, properties.ExpandRowOnOffset);
            var result = new Bitmap(textData.ImageWidth, textData.ImageHeight);

            if (properties.Matte.HasValue)
                result = ImageHelper.Fill(result, properties.Matte.Value);

            var cursor = new Cursor(textData.Padding.Left, textData.Padding.Top);
            int lowerBound = 0; // the largest gap on the current row.
            bool hasOffset = false; // is set to true if any of the characters have a y offset.

            using (Graphics graphics = Graphics.FromImage(result))
            {
                // TO_DO: create an auto line break if the next char to be placed goes outside of the maximum width.
                foreach (CharData c in textData.Chars)
                {
                    if (c.IsNewline)
                    {
                        if (properties.ExpandRowOnOffset && hasOffset)
                        {
                            cursor.Y += lowerBound;
                            hasOffset = false;
                            lowerBound = 0; // resets the largest gap from the previous row.
                        }

                        // the height already calculates for font height and padding.
                        // + 1: Pads the bottom with a graceful pixel.

                        cursor.Y += c.Size.Height;
                        cursor.X = textData.Padding.Left;

                        // this is where any y offsets would also be set if the row is stretched on offsets.

                        continue;
                    }

                    // sets the pointer to the position specified by the character.
                    cursor.Set(c.Position.X, c.Position.Y);

                    /* handle offsets here: you want to put the cursor in the right spot before placing the image. */
                    // this also handles if offsets stretch out the next line.
                    if (c.Offset.Y > 0)
                    {
                        hasOffset = true;

                        cursor.Y += c.Offset.Y;

                        if (c.Offset.Y > lowerBound)
                            lowerBound = c.Offset.Y; // sets the new low
                    }

                    if (c.HasSprite())
                    {
                        using Bitmap sprite = c.GetSprite();
                        ImageHelper.ClipAndDrawImage(graphics, sprite, cursor, c.Size);
                    }

                    cursor.X += c.Size.Width + c.Padding.Right; // this already accounts for width/padding.

                    // Something's not right here
                    if (c.Offset.Y > 0) // this catches negative offsets..?
                        cursor.Y -= c.Offset.Y; // just in case there was an offset.
                }
            }

            // NOTE: The text color is specified here
            result = ImageHelper.SetColorMap(result, ImageHelper.CreateColorTable((ImmutableColor.Default, color)));
            // TODO: Handle text outlining
            textData.Dispose();

            return result;
        }

        public void Dispose()
        {
            if (Disposed)
                return;

            foreach (CachedChar cached in Cache.Values)
                cached.Dispose();

            Disposed = true;
        }
    }
}
