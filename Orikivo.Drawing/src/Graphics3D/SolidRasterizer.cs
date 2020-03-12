using System;
using System.Collections.Generic;
using System.Drawing;
using Point = System.Drawing.Point;

namespace Orikivo.Drawing.Graphics3D
{
    // TODO: Finish solid rasterization.
    public class SolidRasterizer : Rasterizer
    {
        public override Grid<Color> Render(in Model model, Camera camera, GammaPen pen)
        {
            Grid<Color> frame = camera.GetScreen();

            for (int i = 0; i < model.Mesh.Triangles.Count; i++)
            {
                Triangle t = Transform(model.Mesh.Triangles[i], model.Transform);

                Vector3 normal = Vector3.Normal(t.Points[0], t.Points[1], t.Points[2]);
                Vector3 diff = Vector3.Subtract(t.Points[0], camera.Position);
                float dp = Vector3.DotProduct(normal, diff);

                if (normal.Z < 0.0f)
                {
                    Console.WriteLine($"[Visible] Dot Product ({dp})");
                    Triangle p = Project(t, camera.GetProjector(), camera.Width, camera.Height);

                    List<Point> visible = camera.GetVisible(p);

                    foreach (Point v in visible)
                        frame.SetValue(pen.Color, v.X, v.Y);
                }
                else
                    Console.WriteLine($"Dot Product ({dp})");
            }

            return frame;
        }
    }
}
