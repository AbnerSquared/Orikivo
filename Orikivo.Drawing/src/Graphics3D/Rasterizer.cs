using System.Drawing;

namespace Orikivo.Drawing.Graphics3D
{
    // this handles the drawing portion of the MeshRenderer
    public abstract class Rasterizer
    {
        public abstract Grid<Color> Render(in Model model, Camera camera, GammaPen pen);

        protected virtual Triangle Transform(in Triangle t, Transform transform)
        {
            Triangle r = new Triangle(t.Points);
            Triangle s = new Triangle(
                Vector3.Transform(r.Points[0], transform),
                Vector3.Transform(r.Points[1], transform),
                Vector3.Transform(r.Points[2], transform));

            return s;
        }

        protected virtual Triangle Project(in Triangle t, MatrixF projector, int width, int height)
        {
            Triangle r = new Triangle(t.Points);

            r.Points[0] = Vector3.Project(r.Points[0],
                projector, width, height);

            r.Points[1] = Vector3.Project(r.Points[1],
                projector, width, height);

            r.Points[2] = Vector3.Project(r.Points[2],
                projector, width, height);

            return r;
        }
    }
}
