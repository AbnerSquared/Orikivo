using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Orikivo
{
    public static class CaptchaEngine
    {
        private static readonly Random _rng = new Random();

        public static Bitmap Generate(string key)
        {
            Bitmap bmp = new Bitmap(200, 100);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                Rectangle rct = new Rectangle(0, 0, bmp.Width, bmp.Height);

                Color backdropColor = GenerateColor();
                Color foregroundColor = GenerateAlphaColor();
                Color fontForeColor = GenerateColor();

                Color darkThemeColor = Color.FromArgb(54, 57, 63);
                int tolerance = 20;

                int rngR = _rng.Next(255 - fontForeColor.R);
                if (rngR < darkThemeColor.R + tolerance && rngR > darkThemeColor.R - tolerance)
                {
                    int spacing = 255 - rngR;
                    if (spacing > tolerance)
                        rngR += tolerance;
                    else if (spacing < tolerance)
                        rngR -= tolerance;
                }
                int rngG = _rng.Next(255 - fontForeColor.G);
                if (rngG < darkThemeColor.G + tolerance && rngG > darkThemeColor.G - tolerance)
                {
                    int spacing = 255 - rngG;
                    if (spacing > tolerance)
                        rngG += tolerance;
                    else if (spacing < tolerance)
                        rngG -= tolerance;
                }
                int rngB = _rng.Next(255 - fontForeColor.B);
                if (rngB < darkThemeColor.B + tolerance && rngB > darkThemeColor.B - tolerance)
                {
                    int spacing = 255 - rngB;
                    if (spacing > tolerance)
                        rngB += tolerance;
                    else if (spacing < tolerance)
                        rngB -= tolerance;
                }
                int distance = (rngR * rngR) + (rngG * rngG) + (rngB * rngB);
                int average = (int) Math.Sqrt(distance / 3);

                int fontBackR = Math.Abs(fontForeColor.R - rngR);
                int fontBackG = Math.Abs(fontForeColor.G - rngG);
                int fontBackB = Math.Abs(fontForeColor.B - rngB);
                Color fontBackColor = Color.FromArgb(fontBackR, fontBackG, fontBackB);//GenerateColor();

                HatchStyle foregroundBrushStyle = GetRandomHatchStyle();
                Brush foregroundBrush = new HatchBrush(foregroundBrushStyle, fontForeColor, Color.Transparent);

                HatchStyle fontBrushStyle = GetRandomHatchStyle();
                Brush fontBrush = new HatchBrush(fontBrushStyle, fontForeColor, fontBackColor);




                //g.Clear(backdropColor);
                g.Clear(Color.Transparent);

                FontFamily family = FontFamily.GenericSansSerif;
                int size = GetRandomFontSize();
                Font font = new Font(family, size);

                Size area = g.MeasureString(key, font).ToSize();
                int midPointHoriz = (bmp.Width / 2) - (area.Width / 2);
                int midPointVert = (bmp.Height / 2) - (area.Height / 2);
                Point center = new Point(midPointHoriz, midPointVert);
                
                /*Bitmap text = new Bitmap(area.Width, area.Height);
                using (Graphics gr = Graphics.FromImage(text))
                {
                    gr.DrawString(key, font, fontBrush, 0, 0);
                }*/

                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                GraphicsPath path = new GraphicsPath();
                path.AddString(key, family, (int)font.Style, size, rct, format);
                Pen pen = new Pen(fontBrush, _rng.Next(1, 72)); 
                Matrix matrix = new Matrix();

                WarpMode mode = WarpMode.Perspective;
                PointF[] warpPoints = GetRandomWarpPoints(bmp.Size);
                matrix.Translate(0, 0);

                if (_rng.Next(0, 1) == 1)
                    path.Widen(pen, matrix);

                path.Warp(warpPoints, rct, matrix, mode, 0);

                //g.DrawImage(text, center);
                g.FillPath(fontBrush, path);
                g.FillRectangle(foregroundBrush, g.ClipBounds);
            }
            
            return bmp;
        }

        private static PointF[] GetRandomWarpPoints(Size size)
            => GetRandomWarpPoints(size.Width, size.Height);

        private static PointF[] GetRandomWarpPoints(int width, int height)
        {
            //float strength = 2.75f;
            PointF[] coordinates =
            {
                new PointF(_rng.Next(width) / (float) (_rng.Next(275, 400) * 0.01), _rng.Next(height) / (float) (_rng.Next(275, 400) * 0.01)),
                new PointF(width - _rng.Next(width) / (float) (_rng.Next(275, 400) * 0.01), _rng.Next(height) / (float) (_rng.Next(275, 400) * 0.01)),
                new PointF(_rng.Next(width) / (float) (_rng.Next(275, 400) * 0.01), height - _rng.Next(height) / (float) (_rng.Next(275, 400) * 0.01)),
                new PointF(width - _rng.Next(width) / (float) (_rng.Next(275, 400) * 0.01), height - _rng.Next(height) / (float) (_rng.Next(275, 400) * 0.01))
            };

            return coordinates;
        }


        private static Font GetRandomFont()
        {
            Font font = new Font(GetRandomFontFamily(), GetRandomFontSize());
            return font;
        }

        private static FontFamily GetRandomFontFamily()
            => FontFamily.Families[_rng.Next(FontFamily.Families.Length)];
        

        private static int GetRandomFontSize()
            => _rng.Next(48, 64);

        private static Brush GetRandomBrush()
        {
            Brush brush = new HatchBrush(GetRandomHatchStyle(), GenerateColor(), GenerateColor());
            return brush;
        }

        private static HatchStyle GetRandomHatchStyle()
        {
            HatchStyle[] styles =
            {
                HatchStyle.BackwardDiagonal,
                //HatchStyle.DashedVertical,
                //HatchStyle.LargeConfetti,
                HatchStyle.Horizontal,
                HatchStyle.Vertical
            };

            return styles[_rng.Next(styles.Length)];
        }

        private static Color GenerateColor()
            => Color.FromArgb(_rng.Next(0, 255), _rng.Next(0, 255), _rng.Next(0, 255));

        private static Color GenerateAlphaColor()
            => Color.FromArgb(_rng.Next(0,255), _rng.Next(0, 255), _rng.Next(0, 255), _rng.Next(0, 255));
    }
}