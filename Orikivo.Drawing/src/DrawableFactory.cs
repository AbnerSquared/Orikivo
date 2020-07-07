using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Orikivo.Drawing
{

    /// <summary>
    /// A custom image factory that handles the creation of <see cref="Drawable"/> instances.
    /// </summary>
    public class DrawableFactory : IDisposable
    {
        private Image _image;

        public DrawableFactory(int width, int height,
            PixelFormat pixelFormat = PixelFormat.Format32bppArgb,
            GraphicsConfig config = null)
        {
            _image = new Bitmap(width, height, pixelFormat);
            config ??= GraphicsConfig.Default;
            Palette = config.Palette;
            CharMap = config.CharMap;
        }
        public DrawableFactory(Image image, GraphicsConfig config = null)
        {
            _image = image;
            config ??= GraphicsConfig.Default;
            _cacheable = config.CanCacheChars;
            FontDirectory = config.FontDirectory;

            Palette = config.Palette;
            CharMap = config.CharMap;
        }

        public DrawableFactory(GraphicsConfig config = null)
        {
            config ??= GraphicsConfig.Default;
            Palette = config.Palette;
            CharMap = config.CharMap;
        }

        public GammaPalette Palette { get; set; }

        

        public CanvasOptions DefaultOptions { get; set; } = null;

        public void SetPalette(GammaPalette palette)
            => Palette = palette;

        

        public Bitmap DrawSolid(GammaColor color, int width, int height)
            => GraphicsUtils.CreateRgbBitmap(new Grid<Color>(width, height, color).Values);

        //public void DrawLine(Point from, Point to, int thickness, GammaColor color)

        public Bitmap DrawGradient(GammaColor from, GammaColor to, int width, int height, AngleF angle)
        {
            Grid<Color> pixels = new Grid<Color>(width, height, from);
           
            for (int i = 0; i < width; i++)
            {
                float strength = RangeF.Convert(0, width, 0.0f, 1.0f, i);
                GammaColor blend = GammaColor.Merge(from, to, strength);
                pixels.SetColumn(i, blend);
            }

            return GraphicsUtils.CreateRgbBitmap(pixels.Values);
        }

        public Bitmap DrawGradient(Dictionary<float, GammaColor> markers, int width, int height, AngleF angle, GradientColorHandling colorHandling = GradientColorHandling.Blend)
        {
            Color first = markers.OrderBy(x => x.Key).First().Value;
            Grid<Color> pixels = new Grid<Color>(width, height, first);

            for (int i = 0; i < width; i++)
            {
                // places the point on a 0.0 to 1.0 scale
                float point = RangeF.Convert(0, width, 0.0f, 1.0f, i);

                pixels.SetColumn(i, GetColorAtPoint(markers, point, colorHandling));

                //Color f = pixels.GetColumn(i)[0];
                //Console.WriteLine($"Column :: {f.A}, {f.R}, {f.G}, {f.B}");
            }

            return GraphicsUtils.CreateRgbBitmap(pixels.Values);
        }

        private static GammaColor GetColorAtPoint(Dictionary<float, GammaColor> markers, float point, GradientColorHandling colorHandling = GradientColorHandling.Blend)
        {
            IEnumerable<float> last = markers.Keys.Where(x => x < point);
            float? lastClosest = last.Count() > 0 ? last
                .OrderBy(x => MathF.Abs(point - x)).First() : (float?)null;

            IEnumerable<float> next = markers.Keys.Where(x => x >= point);
            float? nextClosest = next.Count() > 0 ? next
                .OrderBy(x => MathF.Abs(point - x)).First() : (float?)null;

            if (lastClosest == null && nextClosest == null)
                throw new Exception("There are no markers specified within the gradient to draw.");

            if (lastClosest == null && nextClosest != null)
                return markers[nextClosest.Value];

            if (lastClosest != null && nextClosest == null)
                return markers[lastClosest.Value];

            GammaColor lastColor = markers[lastClosest.Value];
            GammaColor nextColor = markers[nextClosest.Value];

            if (colorHandling == GradientColorHandling.Snap)
                return lastClosest.Value < nextClosest.Value ? lastColor : nextColor;

            // blending
            // dithering is handled when drawing the gradient.

            float strength = RangeF.Convert(lastClosest.Value, nextClosest.Value, 0.0f, 1.0f, point);

            GammaColor result = GammaColor.Merge(lastColor, nextColor, strength);
            //Console.WriteLine($"Merge ({strength}): {result.A}, {result.R}, {result.G}, {result.B}");
            return result;
        }

        // gets the opacity mask based off of the alpha component of each pixel in the image.
        public Grid<float> GetOpacityMask(Bitmap image)
        {
            Grid<float> mask = new Grid<float>(image.Size, 0.0f);

            Grid<Color> pixels = GraphicsUtils.GetPixels(image);

            mask.SetEachValue((int x, int y, float z) => RangeF.Convert(0, 255, 0.0f, 1.0f, pixels.GetValue(x, y).A));

            return mask;
        }

        public Bitmap SetOpacityMask(Bitmap image, Grid<float> mask)
        {
            if (mask.Size != image.Size)
                throw new ArgumentException("The mask must be the same size as the binding image.");

            Grid<Color> pixels = GraphicsUtils.GetPixels(image);

            pixels.SetEachValue((int x, int y, Color z) =>
                Color.FromArgb((int) MathF.Floor(RangeF.Convert(0.0f, 1.0f, 0.0f, 255.0f, mask.GetValue(x, y))),
                    z));

            return GraphicsUtils.CreateArgbBitmap(pixels.Values);
        }

        // use Dither as default when you figure it out.
        // TODO: handle angles later on
        public Bitmap DrawGradient(GammaPalette palette, int width, int height, AngleF angle, GradientColorHandling colorHandling = GradientColorHandling.Snap)
        {
            Dictionary<float, GammaColor> markers = new Dictionary<float, GammaColor>();
            float markerDist = 1.0f / GammaPalette.RequiredLength;

            for (int i = 0; i < GammaPalette.RequiredLength; i++)
                markers.Add(markerDist * i, palette[(Gamma)i]);

            return DrawGradient(markers, width, height, angle, colorHandling);
        }

        public Bitmap DrawFillable(GammaColor background, GammaColor foreground, int width, int height, float progress, AngleF angle)
        {
            float a = RangeF.Convert(0.0f, 1.0f, 0.0f, width, progress); // x fill

            // TODO: account for direction, using a 2D rotation matrix???
            Bitmap result = new Bitmap(width, height);

            // TODO: Utilize Grid<Color> to create the fillable, due to the angle markers.
            using (Graphics g = Graphics.FromImage(result))
            {
                int fillWidth = (int)Math.Floor(a);

                Rectangle fillClip = new Rectangle(0, 0, fillWidth, height);
                using (SolidBrush fore = new SolidBrush(foreground))
                {
                    g.SetClip(fillClip);
                    g.FillRectangle(fore, fillClip);
                    g.ResetClip();
                }

                Rectangle emptyClip = new Rectangle(fillWidth, 0, width - fillWidth, height);
                using (SolidBrush back = new SolidBrush(background))
                {
                    g.SetClip(emptyClip);
                    g.FillRectangle(back, emptyClip);
                    g.ResetClip();
                }
            }

            return result;
        }

        public Bitmap DrawBorder(Bitmap image, GammaColor color, int width, BorderEdge edge = BorderEdge.Outside)
        {
            // drawing outside
            int innerWidth = GetInnerWidth(width, edge);
            int outerWidth = GetOuterWidth(width, edge);

            // TODO: use the Grid<T> system, as editing Bitmaps can be costly. It might be better to simply edit list values, instead of pixels.
            //Grid<GammaColor> result = new Grid<GammaColor>(image.Width + outerWidth * 2, image.Height + outerWidth * 2, new GammaColor(0, 0, 0, 0));
            Bitmap result = new Bitmap(image.Width + (outerWidth * 2), image.Height + (outerWidth * 2));

            if (!(width > 0))
                return image;

            using (Graphics g = Graphics.FromImage(result))
            {
                SolidBrush brush = new SolidBrush(color);
                if (outerWidth > 0)
                {
                    Point imagePos = new Point(outerWidth * 2, outerWidth * 2);
                    Size v = new Size(outerWidth, image.Height + (outerWidth * 2)); // for now, just make square borders, but next time, make it so that corners can be drawn.
                    Size h = new Size(image.Width, outerWidth);

                    Point l = new Point(0, 0);
                    Point r = new Point(image.Width + outerWidth, 0);
                    Point t = new Point(outerWidth, 0);
                    Point b = new Point(outerWidth, image.Height + outerWidth);

                    Rectangle left = new Rectangle(l, v);
                    Rectangle right = new Rectangle(r, v);
                    Rectangle top = new Rectangle(t, h);
                    Rectangle bottom = new Rectangle(b, h);

                    g.FillRectangles(brush, new Rectangle[] { left, right, top, bottom });
                }

                if (innerWidth > 0)
                {
                    int maxWidth = (int)Math.Floor((double)image.Width / 2);

                    if (innerWidth >= maxWidth)
                        g.FillRectangle(brush, new Rectangle(0, 0, image.Width, image.Height));
                    else
                    {
                        var v = new Size(innerWidth, image.Height);
                        var h = new Size(image.Width - (innerWidth * 2), innerWidth); // make inner corners possible

                        var l = new Point(0, 0);
                        var t = new Point(innerWidth, 0);
                        var r = new Point(image.Width - innerWidth, 0);
                        var b = new Point(innerWidth, image.Height - innerWidth);

                        Rectangle left = new Rectangle(l, v);
                        Rectangle right = new Rectangle(r, v);
                        Rectangle top = new Rectangle(t, h);
                        Rectangle bottom = new Rectangle(b, h);

                        g.FillRectangles(brush, new Rectangle[] { left, right, top, bottom });
                    }
                }

                brush.Dispose();

                GraphicsUtils.ClipAndDrawImage(g, image, new Point(outerWidth, outerWidth));
            }

            return result;
        }

        private static int GetInnerWidth(int width, BorderEdge edge)
            => edge switch
            {
                BorderEdge.Outside => 0,
                BorderEdge.Center => (int)Math.Floor((double)width / 2),
                BorderEdge.Inside => width,
                _ => 0
            };

        private static int GetOuterWidth(int width, BorderEdge edge)
            => edge switch
            {
                BorderEdge.Outside => width,
                BorderEdge.Center => width - (int)Math.Floor((double)width / 2),
                BorderEdge.Inside => 0,
                _ => width
            };

        // all of this needs to be handled by the TextFactory

        private readonly bool _cacheable = true;
        private static readonly string _defaultFontDirectory = "../assets/fonts/";
        private char[][][][] CharMap { get; }
        private string FontDirectory { get; }

        private Dictionary<char, Bitmap> CharCache { get; set; } = new Dictionary<char, Bitmap>();

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

        public void SetFont(FontFace font)
        {
            ImportFont(font);
            CurrentFontIndex = Fonts.IndexOf(font);
        }

        public void ImportFont(string path)
            => ImportFont(FontFace.FromPath(path));

        public void ImportFont(FontFace font)
        {
            if (!Fonts.Contains(font))
                Fonts.Add(font);
        }

        public Bitmap GetChar(char value, FontFace font = null)
        {
            font ??= CurrentFont;

            if (_cacheable)
                if (CharCache.ContainsKey(value))
                    return CharCache[value];

            Bitmap bmp = GetRawChar(value, font);

            if (bmp == null)
                return bmp;

            if (!font.IsMonospace)
                return ImageHelper.Crop(bmp, 0, 0, ImageHelper.GetNonEmptyWidth(bmp), font.CharHeight);

            if (_cacheable)
                CharCache[value] = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), bmp.PixelFormat);

            return bmp;
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

        internal Bitmap GetChar(char c, FontFace font = null, bool useNonEmptyWidth = true)
        {
            font ??= CurrentFont;

            if (_cacheable)
                if (CharCache.ContainsKey(c))
                    return CharCache[c];

            Bitmap bmp = GetRawChar(c, font);

            if (bmp == null)
                return bmp;

            if (!font.IsMonospace && useNonEmptyWidth)
            {
                return ImageHelper.Crop(bmp, new Rectangle(0, 0, ImageHelper.GetNonEmptyWidth(bmp), font.CharHeight));
            }

            if (_cacheable)
                CharCache[c] = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), bmp.PixelFormat);

            return bmp;
        }

        private Dictionary<char, Bitmap> GetChars(string content, FontFace font = null, bool useNonEmptyWidth = true)
        {
            font ??= CurrentFont;
            char[] chars = content.ToCharArray();

            if (!(chars?.Length > 0))
                throw new Exception("One char must be specified at minimum.");

            chars = chars.Where(x => !WhiteSpaceInfo.IsWhiteSpace(x) && x != '\n').ToArray();

            List<char> existingChars = new List<char>();

            foreach (char c in chars)
            {
                if (existingChars.Contains(c))
                    continue;

                existingChars.Add(c);
            }

            chars = existingChars.ToArray();

            Dictionary<char, Bitmap> charMap = new Dictionary<char, Bitmap>();

            for (int i = 0; i < chars.Length; i++)
            {
                if (charMap.ContainsKey(chars[i]))
                    continue;

                charMap[chars[i]] = GetChar(chars[i], font, useNonEmptyWidth);
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
        private TextData CreateText(Dictionary<char, Bitmap> spriteMap, string content, FontFace font, Padding? textPadding = null, int? maxWidth = null, int? maxHeight = null, bool useNonEmptyWidth = true, bool extendOnOffset = false)
        {
            bool autoWidth = useNonEmptyWidth && !font.IsMonospace;
            Padding padding = textPadding ?? Padding.Empty;


            if (!extendOnOffset)
                extendOnOffset = font.Overrides?.Any(x => x.GetOffset() != null) ?? extendOnOffset;

            Pointer cursor = new Pointer(maxWidth: maxWidth, maxHeight: maxHeight);

            List<char> chars = content.ToList();
            List<CharData> charObjects = new List<CharData>();

            int cursorHeight = font.Padding.Height + font.CharHeight;
            int yMaxOffset = 0;
            int charIndex = 0;

            foreach (char c in chars)
            {
                // Calculate the character's width and padding
                Padding charPadding = font.Padding;

                int? spriteWidth = spriteMap.GetValueOrDefault(c)?.Width;

                int drawWidth = font.GetCharWidth(c);

                if (autoWidth && spriteWidth.HasValue)
                    drawWidth = spriteWidth.Value;

                int cursorWidth = font.Padding.Width + drawWidth;

                // LINE BREAKS
                if (c == '\n')
                {
                    int pX = padding.Left + cursor.X;
                    int pY = padding.Top + cursor.Y;

                    charObjects.Add(new CharData(null, c, pX, pY, 0, font.CharHeight));

                    cursor.ResetX();
                    cursor.MoveY(cursorHeight + 1);
                    charIndex++;
                    continue;
                }

                // EMPTY CHARS
                if (WhiteSpaceInfo.IsWhiteSpace(c))
                {
                    int emptyWidth = autoWidth ? drawWidth
                        : font.CharWidth + font.Padding.Width;

                    int pX = padding.Left + cursor.X;
                    int pY = padding.Top + cursor.Y;

                    charObjects.Add(new CharData(null, c, pX, pY, emptyWidth, font.CharHeight));

                    cursor.MoveX(emptyWidth);
                    charIndex++;
                    continue;
                }

                yMaxOffset += Math.Max(font.GetCharOffset(c).Y, 0);

                bool hasCharBefore = chars.TryGetElementAt(charIndex - 1, out char beforeChar);
                bool hasCharAfter = chars.TryGetElementAt(charIndex + 1, out char afterChar);

                if (autoWidth)
                {
                    if (charIndex == 0) // if it's the 1st character, no padding on the left
                    {
                        cursorWidth -= font.Padding.Left;
                        charPadding.Left = 0;
                    }
                    else if (hasCharBefore)
                    {
                        cursorWidth -= font.Padding.Left;
                        charPadding.Left = 0;
                    }

                    if (charIndex == chars.Count - 1) // if it's the last character, no padding on the right
                    {
                        cursorWidth -= font.Padding.Right;
                        charPadding.Right = 0;
                    }
                    else if (hasCharAfter)
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
                    charObjects.Add(
                        new CharData(
                            spriteMap[c],
                            c,
                            new Point(padding.Left + cursor.X, padding.Top + cursor.Y),
                            new Size(drawWidth, font.CharHeight),
                            font.Padding,
                            font.GetCharOffset(c)));

                    cursor.MoveX(cursorWidth);
                }

                charIndex++;
            }

            int height = cursor.Height + font.CharHeight;

            if (extendOnOffset)
                height += yMaxOffset;  // if extending on offsets, add it to the total height.

            return new TextData(content, padding, cursor.Width, height, charObjects);
        }

        public Bitmap DrawString(string content, CanvasOptions options = null)
            => DrawString(content, CurrentFont, GammaPalette.Default[Gamma.Max], options);

        public Bitmap DrawString(string content, FontFace font, CanvasOptions options = null)
            => DrawString(content, font, GammaPalette.Default[Gamma.Max], options);

        public Bitmap DrawString(string content, Color color, CanvasOptions options = null)
            => DrawString(content, CurrentFont, color, options);

        public Bitmap DrawString(string content, FontFace font, Color color, CanvasOptions options = null)
        {
            options ??= new CanvasOptions();
            TextData textData = CreateText(GetChars(content, font, options?.UseNonEmptyWidth ?? true),
                content,
                font,
                options?.Padding,
                useNonEmptyWidth: options?.UseNonEmptyWidth ?? true,
                extendOnOffset: options?.ExtendOnOffset ?? false);

            Bitmap bmp = new Bitmap(textData.ImageWidth, textData.ImageHeight);

            if (options?.BackgroundColor.HasValue ?? false)
                bmp = ImageHelper.Fill(bmp, options.BackgroundColor.Value);

            var cursor = new Cursor(textData.Padding.Left, textData.Padding.Top);

            int yOffset = 0; // the largest y offset in place.

            bool hasOffset = false; // is set to true if any of the characters have a y offset.

            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                // TO_DO: create an auto line break if the next char to be placed goes outside of the maximum width.
                foreach (CharData c in textData.Chars)
                {
                    if (c.IsNewline)
                    {
                        if (options?.ExtendOnOffset ?? false && hasOffset)
                        {
                            cursor.Y += yOffset;

                            hasOffset = false;

                            yOffset = 0; // resets the largest offset from the previous row.
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

                        if (c.Offset.Y > yOffset)
                            yOffset = c.Offset.Y; // sets the new largest offset.
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

            // dispose of TextData

            textData.Dispose();

            return bmp;
        }

        public bool Disposed { get; private set; } = false;

        // gets rid of all rendered objects
        public void Dispose()
        {
            if (Disposed)
                return;

            if (!_cacheable)
                return;

            foreach (Bitmap bmp in CharCache.Values)
                bmp.Dispose();

            Disposed = true;
        }
    }
}
