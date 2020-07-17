using Orikivo.Drawing;
using Orikivo.Drawing.Graphics2D;
using System;
using System.Drawing;

namespace Orikivo.Desync
{
    // NOTE: Engine.Rendering will handle all rendering methods
    public static partial class Engine
    {
        public static Bitmap DrawMap(string id, HuskBrain brain, GammaPalette palette)
        {
            Map map = GetMap(id, brain);
            var result = new Drawable(map.Source.Width, map.Source.Height);
            Grid<Color> mask = CreateMapMask(map.Progression, Color.Transparent, GammaPalette.Default[Gamma.Min]);

            result.Palette = palette;
            result.AddLayer(new BitmapLayer(ImageEditor.CreateArgbBitmap(mask.Values)));

            return result.BuildAndDispose();
        }

        private static Grid<Color> CreateMapMask(Grid<bool> values, Color on, Color off)
            => values.Select(x => x ? on : off);

        // This draws all hitboxes in a sector.
        private static Grid<Color> DrawSector(Husk husk, Sector sector)
        {
            var canvas = CreateCanvas(sector.Shape, GammaPalette.GammaGreen[Gamma.Min]);

            DrawHitboxOn(canvas,
                GetHitbox(husk),
                GammaPalette.GammaGreen[Gamma.Standard],
                GammaPalette.GammaGreen[Gamma.StandardDim],
                GammaPalette.Alconia[Gamma.Min]);

            if (sector.Regions?.Count > 0)
            {
                foreach (Region region in sector.Regions)
                    DrawRectangleOn(canvas, region.Shape, GammaPalette.GammaGreen[Gamma.Dim]);
            }

            foreach (Area area in sector.Areas)
            {
                DrawRectangleOn(canvas, area.Shape, GammaPalette.GammaGreen[Gamma.Bright]);

                if (area.Entrances?.Count > 0)
                {
                    foreach (Vector2 entrance in area.Entrances)
                        DrawPointOn(canvas, entrance, GammaPalette.GammaGreen[Gamma.Max]);
                }
            }

            foreach (Structure structure in sector.Structures)
                DrawRectangleOn(canvas, structure.Shape, GammaPalette.NeonRed[Gamma.Max]);

            return canvas.Pixels;
        }

        // TODO: Finish this. This should make imagining the visual mapping of a world much easier.
        //       and allows you to see how the constructor for regions and stuff works.
        public static Bitmap DebugDraw(Location location)
        {
            throw new NotImplementedException("This method has not yet been implemented");
        }

        public static Grid<Color> DebugDraw(Sector sector, Husk husk)
        {
            return DrawSector(husk, sector);
        }

        private static void DrawHitboxOn(Canvas canvas, EntityHitbox hitbox, ImmutableColor sightColor, ImmutableColor reachColor, ImmutableColor originColor)
        {
            DrawCircleOn(canvas, hitbox.Sight, sightColor);
            DrawCircleOn(canvas, hitbox.Reach, reachColor);
            DrawPointOn(canvas, hitbox.X, hitbox.Y, originColor);
        }

        private static Canvas CreateCanvas(RegionF region, ImmutableColor backgroundColor)
            => CreateCanvas(region.Width, region.Height, backgroundColor);

        private static Canvas CreateCanvas(float width, float height, ImmutableColor backgroundColor)
        {
            int u = (int)MathF.Floor(width);
            int v = (int)MathF.Floor(height);

            return new Canvas(u, v, backgroundColor);
        }

        private static void DrawCircleOn(Canvas canvas, CircleF circle, ImmutableColor color)
            => DrawCircleOn(canvas, circle.X, circle.Y, circle.Radius, color);

        private static void DrawCircleOn(Canvas canvas, float x, float y, float radius, ImmutableColor color)
        {
            int u = (int)MathF.Floor(x);
            int v = (int)MathF.Floor(y);
            int r = (int)MathF.Floor(radius);

            canvas.DrawCircle(u, v, r, GammaPalette.GammaGreen[Gamma.Standard]);
        }

        private static void DrawPointOn(Canvas canvas, Vector2 point, ImmutableColor color)
            => DrawPointOn(canvas, point.X, point.Y, color);

        private static void DrawPointOn(Canvas canvas, float x, float y, ImmutableColor color)
        {
            int u = (int)MathF.Floor(x);
            int v = (int)MathF.Floor(y);

            if (canvas.Pixels.Contains(u, v))
                canvas.Pixels.SetValue(color, u, v);
        }

        private static void DrawLineOn(Canvas canvas, Line line, ImmutableColor color)
            => DrawLineOn(canvas, line.A.X, line.A.Y, line.B.X, line.B.Y, color);

        private static void DrawLineOn(Canvas canvas, Vector2 a, Vector2 b, ImmutableColor color)
            => DrawLineOn(canvas, a.X, a.Y, b.X, b.Y, color);

        private static void DrawLineOn(Canvas canvas, float ax, float ay, float bx, float by, ImmutableColor color)
            => canvas.DrawLine((int)MathF.Floor(ax), (int)MathF.Floor(ay), (int)MathF.Floor(bx), (int)MathF.Floor(by), color);

        private static void DrawRectangleOn(Canvas canvas, RegionF region, ImmutableColor color)
        {
            RegionF flat = RegionF.Floor(region);
            canvas.DrawRectangle((int)flat.X, (int)flat.Y, (int)flat.Width, (int)flat.Height, color);
        }

        private static void DrawRectangleOn(Canvas canvas, float x, float y, float width, float height, ImmutableColor color)
            => canvas.DrawRectangle((int)MathF.Floor(x),
                (int)MathF.Floor(y),
                (int)MathF.Floor(width),
                (int)MathF.Floor(height),
                color);
    }
}
