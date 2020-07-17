using System;
using System.Collections.Generic;
using System.Drawing;
using Point = System.Drawing.Point;

namespace Orikivo.Drawing.Graphics3D
{
    public class Camera
    {
        public Camera(int width, int height, float fov, float near, float far, GammaPalette palette)
        {
            Width = width;
            Height = height;
            Fov = fov;
            Near = near;
            Far = far;
            Position = Vector3.Zero;
            Rotation = Vector3.Zero;
            Palette = palette;
            BackgroundColor = palette[Gamma.Min];
        }

        public GammaPalette Palette { get; set; }

        public ImmutableColor BackgroundColor { get; set; }

        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public float Fov { get; set; }

        // float ViewDistance
        // ClipPlane NearClipPlane
        // ClipPlane FarClipPlane

        public float Far { get; set; }
        public float Near { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public Grid<Color> GetScreen()
            => new Grid<Color>(Width, Height, BackgroundColor);

        public MatrixF GetProjector()
            => MatrixF.CreateProjector(Near, Far, Fov, Width / Height);

        public bool Contains(Point p)
            => Contains(p.X, p.Y);

        public bool Contains(int x, int y)
        {
            return RangeF.Contains(0, Width, x, true, false) &&
                   RangeF.Contains(0, Height, y, true, false);
        }

        public List<Point> GetVisible(Point a, Point b)
            => GetVisible(a.X, a.Y, b.X, b.Y);

        public List<Point> GetVisible(int x1, int y1, int x2, int y2)
        {
            List<Point> points = new List<Point>();
            int x = 0;
            int y = 0;

            int endX = 0;
            int endY = 0;

            int dx = x2 - x1;
            int dy = y2 - y1;
            int run = Math.Abs(dx);
            int rise = Math.Abs(dy);

            int px = 2 * rise - run;
            int py = 2 * run - rise;

            bool both = (dx < 0 && dy < 0) || (dx > 0 && dy > 0);

            if (rise <= run)
            {
                x = dx >= 0 ? x1 : x2;
                y = dx >= 0 ? y1 : y2;
                endX = dx >= 0 ? x2 : x1;

                if (Contains(x, y))
                    points.Add(new Point(x, y));

                while (x < endX)
                {
                    x++;

                    if (px < 0)
                    {
                        px += 2 * rise;
                    }
                    else
                    {
                        y += both ? 1 : -1;
                        px += 2 * (rise - run);
                    }

                    if (Contains(x, y))
                        points.Add(new Point(x, y));
                }
            }
            else
            {
                x = dy >= 0 ? x1 : x2;
                y = dy >= 0 ? y1 : y2;
                endY = dy >= 0 ? y2 : y1;

                if (Contains(x, y))
                    points.Add(new Point(x, y));

                while (y < endY)
                {
                    y++;

                    if (py <= 0)
                    {
                        py += 2 * run;
                    }
                    else
                    {
                        x += both ? 1 : -1;
                        py += 2 * (run - rise);
                    }

                    if (Contains(x, y))
                        points.Add(new Point(x, y));
                }
            }

            return points;
        }

        public List<Point> GetVisible(Triangle t)
        {
            Point a = new Point((int)MathF.Round(t.Points[0].X), (int)MathF.Round(t.Points[0].Y));
            Point b = new Point((int)MathF.Round(t.Points[1].X), (int)MathF.Round(t.Points[1].Y));
            Point c = new Point((int)MathF.Round(t.Points[2].X), (int)MathF.Round(t.Points[2].Y));

            List<Point> points = new List<Point>();

            points.AddRange(GetVisible(a.X, a.Y, b.X, b.Y));
            points.AddRange(GetVisible(b.X, b.Y, c.X, c.Y));
            points.AddRange(GetVisible(c.X, c.Y, a.X, a.Y));

            return points;
        }
    }
}
