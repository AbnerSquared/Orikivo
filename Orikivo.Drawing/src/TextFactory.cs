using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Orikivo.Drawing
{
    public class TextFactory : IDisposable
    {
        private readonly bool _cacheable = true;
        private static readonly string _defaultFontDirectory = "../assets/fonts/";

        private char[][][][] CharMap { get; }

        private Dictionary<char, Bitmap> Cache { get; set; } = new Dictionary<char, Bitmap>();

        public List<FontFace> Fonts { get; private set; } = new List<FontFace>();

        public FontFace CurrentFont => Fonts[_currentFontIndex];

        private int _currentFontIndex = 0;

        public int CurrentFontIndex
        {
            get => _currentFontIndex;
            set
            {
                if (value >= Fonts.Count || value == 0)
                    return;

                _currentFontIndex = value;
            }
        }

        public bool Disposed { get; private set; } = false;

        public void ImportFont(FontFace font)
        {
            if (!Fonts.Contains(font))
                Fonts.Add(font);
        }

        public void SetFont(FontFace font)
        {
            ImportFont(font);
            CurrentFontIndex = Fonts.IndexOf(font);
        }

        internal Bitmap GetRawChar(char c, FontFace font = null)
        {
            font ??= CurrentFont;

            if (WhiteSpaceInfo.IsWhiteSpace(c) || c == '\n')
                return null;

            CharMapIndex i = GraphicsUtils.GetCharIndex(c, CharMap);

            if (!i.IsSuccess || !font.SheetUrls.Keys.Contains(i.Page))
            {
                // if (!font.HideBadUnicode) TODO: This should return the UNKNOWN unicode sprite.
                //    return new Bitmap(); 
                // else
                return null;
            }

            using (Bitmap bmp = new Bitmap($"{_defaultFontDirectory}{font.SheetUrls[i.Page]}")) // TODO: Handle path assignment
            {
                int x = font.CharWidth * i.Column;
                int y = font.CharHeight * i.Row;

                Rectangle crop = new Rectangle(x, y, font.CharWidth, font.CharHeight);
                Bitmap tmp = ImageHelper.Crop(bmp, crop);

                return tmp;
            }
        }


        public Bitmap GetChar(char value, FontFace font = null, bool trimEmptyPixels = true)
        {
            font ??= CurrentFont;

            if (_cacheable)
                if (Cache.ContainsKey(value))
                    return Cache[value];

            Bitmap bmp = GetRawChar(value, font);

            if (bmp == null)
                return bmp;

            if (_cacheable)
                Cache[value] = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), bmp.PixelFormat);

            if (!font.IsMonospace && trimEmptyPixels)
                return ImageHelper.Crop(bmp, 0, 0, ImageHelper.GetNonEmptyWidth(bmp), font.CharHeight);

            return bmp;
        }

        private Dictionary<char, Bitmap> GetChars(string content, FontFace font = null, bool trimEmptyPixels = true)
        {
            font ??= CurrentFont;

            if (string.IsNullOrWhiteSpace(content))
                return null;

            char[] chars = content.ToCharArray()
                .Where(x => !WhiteSpaceInfo.IsWhiteSpace(x) && x != '\n').ToArray();

            var charMap = new Dictionary<char, Bitmap>();

            for (int i = 0; i < chars.Length; i++)
            {
                if (charMap.ContainsKey(chars[i]))
                    continue;

                charMap[chars[i]] = GetChar(chars[i], font, trimEmptyPixels);
            }

            return charMap;
        }

        // make two versions, one with rendered sprites, and one without.
        // store the sprites and length at the same time to reduce redraw time.
        // make .MaxHeight .MaxWidth .Width .Height
        // if Width is specified, the canvas will be that width regardless
        // if .MaxWidth is specified instead, the canvas can expand up to that width.
        // TODO: Merge GetChars() and CreateText() together.
        // TODO: Scrap AutoWidth, and simply use IsMonospace
        private TextData CreateText(Dictionary<char, Bitmap> spriteMap, string content, FontFace font, Padding? imagePadding = null, int? maxWidth = null, int? maxHeight = null, bool trimEmptyPixels = true, bool extendOnOffset = false)
        {
            bool canTrim = trimEmptyPixels && !font.IsMonospace;
            Padding padding = imagePadding ?? Padding.Empty;

            if (!extendOnOffset)
                extendOnOffset = font.Overrides?.Any(x => x.GetOffset() != null) ?? extendOnOffset;

            Pointer cursor = new Pointer(maxWidth: maxWidth, maxHeight: maxHeight);

            List<char> chars = content.ToList();
            List<CharData> charData = new List<CharData>();

            int cursorHeight = font.Padding.Height + font.CharHeight;
            int yMaxOffset = 0;
            int charIndex = 0;

            foreach (char c in chars)
            {
                // Calculate the character's width and padding
                Padding charPadding = font.Padding;

                int? spriteWidth = spriteMap.GetValueOrDefault(c)?.Width;

                int drawWidth = font.GetCharWidth(c);

                if (canTrim && spriteWidth.HasValue)
                    drawWidth = spriteWidth.Value;

                int cursorWidth = font.Padding.Width + drawWidth;

                // LINE BREAKS
                if (c == '\n')
                {
                    int pX = padding.Left + cursor.X;
                    int pY = padding.Top + cursor.Y;

                    charData.Add(new CharData(null, c, pX, pY, 0, font.CharHeight));

                    cursor.ResetX();
                    cursor.MoveY(cursorHeight + 1);
                    charIndex++;
                    continue;
                }

                // EMPTY CHARS
                if (WhiteSpaceInfo.IsWhiteSpace(c))
                {
                    int emptyWidth = canTrim ? drawWidth
                        : font.CharWidth + font.Padding.Width;

                    int pX = padding.Left + cursor.X;
                    int pY = padding.Top + cursor.Y;

                    charData.Add(new CharData(null, c, pX, pY, emptyWidth, font.CharHeight));

                    cursor.MoveX(emptyWidth);
                    charIndex++;
                    continue;
                }

                yMaxOffset += Math.Max(font.GetCharOffset(c).Y, 0);


                if (canTrim)
                {
                    if (charIndex == 0) // if it's the 1st character, no padding on the left
                    {
                        cursorWidth -= font.Padding.Left;
                        charPadding.Left = 0;
                    }
                    else if (charIndex == chars.Count - 1) // if it's the last character, no padding on the right
                    {
                        cursorWidth -= font.Padding.Right;
                        charPadding.Right = 0;
                    }

                    bool hasCharBefore = chars.TryGetElementAt(charIndex - 1, out char beforeChar);
                    bool hasCharAfter = chars.TryGetElementAt(charIndex + 1, out char afterChar);

                    if (hasCharBefore)
                    {
                        cursorWidth -= font.Padding.Left;
                        charPadding.Left = 0;
                    }

                    if (hasCharAfter)
                    {
                        if (afterChar == '\n' || WhiteSpaceInfo.IsWhiteSpace(afterChar))
                        {
                            cursorWidth -= font.Padding.Right;
                            charPadding.Right = 0;
                        }
                    }
                }

                if (spriteMap[c] != null)
                {
                    int pX = padding.Left + cursor.X;
                    int pY = padding.Top + cursor.Y;

                    charData.Add(new CharData(spriteMap[c], c, pX, pY, drawWidth, font.CharHeight, font.Padding, font.GetCharOffset(c)));
                    cursor.MoveX(cursorWidth);
                }

                charIndex++;
            }

            int height = cursor.Height + font.CharHeight;

            if (extendOnOffset)
                height += yMaxOffset;  // if extending on offsets, add it to the total height.

            return new TextData(content, padding, cursor.Width, height, charData);
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
            TextData textData = CreateText(GetChars(content, font, properties.TrimEmptyPixels),
                content,
                font,
                properties.Padding,
                properties.Width,
                properties.Height,
                properties.TrimEmptyPixels,
                properties.ExpandRowOnOffset);

            var bmp = new Bitmap(textData.ImageWidth, textData.ImageHeight);

            if (properties.Matte.HasValue)
                bmp = ImageHelper.Fill(bmp, properties.Matte.Value);

            var cursor = new Cursor(textData.Padding.Left, textData.Padding.Top);
            int lowerBound = 0; // the largest gap on the current row.
            bool hasOffset = false; // is set to true if any of the characters have a y offset.

            using (Graphics graphics = Graphics.FromImage(bmp))
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
                        using (Bitmap sprite = c.GetSprite()) // if the sprite exists, use it to place and dispose.
                            GraphicsUtils.ClipAndDrawImage(graphics, sprite, new Rectangle(cursor, c.Size));
                    }

                    cursor.X += c.Size.Width + c.Padding.Right; // this already accounts for width/padding.

                    if (c.Offset.Y > 0) // this catches negative offsets..?
                        cursor.Y -= c.Offset.Y; // just in case there was an offset.
                }
            }

            // recolor bitmap here, and handle outlines here.

            bmp = ImageHelper.SetColorMap(bmp, ImageHelper.CreateColorTable((Color.White, color)));
            textData.Dispose();

            return bmp;
        }

        public void Dispose()
        {
            if (Disposed)
                return;

            foreach (Bitmap bmp in Cache.Values)
                bmp.Dispose();

            Disposed = true;
        }
    }
}
