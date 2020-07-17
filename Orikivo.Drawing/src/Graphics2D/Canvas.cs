using System;
using System.Collections.Generic;
using System.Drawing;

namespace Orikivo.Drawing.Graphics2D
{
    public class Canvas
    {
        public Canvas(int width, int height, Color backgroundColor)
        {
            Pixels = new Grid<Color>(width, height, backgroundColor);
        }

        public Grid<Color> Pixels { get; set; }

        // NOTE: This is the active pen used to draw when active
        public Pen Pen { get; set; }

        public void PenDown()
        {
            Pen.IsDown = true;
        }

        public void PenUp()
        {
            Pen.IsDown = false;
        }

        public void Clear(Color color)
        {
            Pixels.Clear(color);
        }

        public void Stamp(Grid<Color> pixels, int x, int y)
        {
            Pixels.SetRegion(pixels, x, y);
        }

        // TODO: Implement Canvas.Stamp(Image)
        //public void Stamp(Image image) { }

        public void DrawCircle(Point origin, int radius, Color color)
            => DrawCircle(origin.X, origin.Y, radius, color);

        // REF: https://en.wikipedia.org/wiki/Midpoint_circle_algorithm
        // REF: https://www.geeksforgeeks.org/mid-point-circle-drawing-algorithm/
        // NOTE: This method draws a circle using the Midpoint Circle Algorithm
        public void DrawCircle(int originX, int originY, int radius, Color color)
        {
            var pixels = GetCirclePixels(originX, originY, radius);

            foreach(Point pixel in pixels)
            {
                if (Pixels.Contains(pixel.X, pixel.Y))
                    Pixels.SetValue(color, pixel.X, pixel.Y);
            }
        }

        public void DrawLine(int ax, int ay, int bx, int by, Color color)
        {
            var pixels = GetLinePixels(ax, ay, bx, by);

            foreach(Point pixel in pixels)
            {
                if (Pixels.Contains(pixel.X, pixel.Y))
                    Pixels.SetValue(color, pixel.X, pixel.Y);
            }
        }

        public void DrawLine(Point a, Point b, Color color)
            => DrawLine(a.X, a.Y, b.X, b.Y, color);

        public void DrawRectangle(int x, int y, int width, int height, Color color)
        {
            var top = GetLinePixels(x, y, x + width, y);
            var left = GetLinePixels(x, y, x, y + height);
            var right = GetLinePixels(x + width, y, x + width, y + height);
            var bottom = GetLinePixels(x, y + height, x + width, y + height);

            var pixels = new List<Point>();

            // TODO: Remove existing points.
            pixels.AddRange(top);
            pixels.AddRange(left);
            pixels.AddRange(right);
            pixels.AddRange(bottom);

            foreach (Point pixel in pixels)
            {
                if (Pixels.Contains(pixel.X, pixel.Y))
                    Pixels.SetValue(color, pixel.X, pixel.Y);
            }

        }

        public void DrawRectangle(Point point, Size size, Color color)
            => DrawRectangle(point.X, point.Y, size.Width, size.Height, color);
        
        public void DrawPoint(int x, int y, Color color)
        {
            if (Pixels.Contains(x, y))
                Pixels.SetValue(color, x, y);
        }

        public List<Point> GetCirclePixels(int originX, int originY, int radius)
        {
            var pixels = new List<Point>();
            // initial points
            int x = radius;
            int y = 0;

            // set initial pixels
            //                     radius, 0
            pixels.Add(new Point(radius + originX, originY));

            if (radius > 0)
            {
                //                     -radius, 0
                pixels.Add(new Point(-radius + originX, originY));

                //                     0, radius
                pixels.Add(new Point(originX, radius + originY));

                //                     0, -radius
                pixels.Add(new Point(originX, -radius + originY));
            }

            int midpoint = 1 - radius;
            while (x > y)
            {
                y++;

                if (midpoint <= 0)
                {
                    midpoint = midpoint + 2 * y + 1;
                }
                else
                {
                    x--;
                    midpoint = midpoint + 2 * y - 2 * x + 1;
                }

                if (x < y)
                {
                    break;
                }

                // NOTE: This is adding all points with their reflections in other octants
                pixels.Add(new Point(x + originX, y + originY));
                pixels.Add(new Point(-x + originX, y + originY));
                pixels.Add(new Point(x + originX, -y + originY));
                pixels.Add(new Point(-x + originX, -y + originY));

                if (x != y)
                {
                    pixels.Add(new Point(y + originX, x + originY));
                    pixels.Add(new Point(-y + originX, x + originY));
                    pixels.Add(new Point(y + originX, -x + originY));
                    pixels.Add(new Point(-y + originX, -x + originY));
                }
            }

            return pixels;
        }

        // REF: https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
        // REF: http://rosettacode.org/wiki/Bitmap/Bresenham%27s_line_algorithm#C.23
        // NOTE: This draws a line using Bresenham's Line algorithm
        public List<Point> GetLinePixels(int ax, int ay, int bx, int by)
        {
            var pixels = new List<Point>();

            int dx = Math.Abs(bx - ax);
            int sx = ax < bx ? 1 : -1;

            int dy = Math.Abs(by - ay);
            int sy = ay < by ? 1 : -1;

            int err = (dx > dy ? dx : -dy) / 2;
            int e2;

            for(;;)
            {
                pixels.Add(new Point(ax, ay));

                if (ax == bx && ay == by)
                {
                    break;
                }

                e2 = err;

                if (e2 > -dx)
                {
                    err -= dy;
                    ax += sx;
                }

                if (e2 < dy)
                {
                    err += dx;
                    ay += sy;
                }
            }

            return pixels;
        }

        public Bitmap Build()
        {
            return ImageEditor.CreateArgbBitmap(Pixels.Values);
        }
    }
}
