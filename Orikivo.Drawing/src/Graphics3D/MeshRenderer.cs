using System.Collections.Generic;
using System.Drawing;

namespace Orikivo.Drawing.Graphics3D
{
    public class MeshRenderer
    {
        public MeshRenderer(Camera camera, Rasterizer rasterizer)
        {
            Camera = camera;
            Rasterizer = rasterizer;
            Objects = new List<Model>();
        }

        public MeshRenderer(int width, int height, float fov, float near, float far, GammaPalette palette, Rasterizer rasterizer)
        {
            Camera = new Camera(width, height, fov, near, far, palette);
            Objects = new List<Model>();
            Rasterizer = rasterizer;
        }

        public Camera Camera { get; set; }
        public Rasterizer Rasterizer { get; }
        public List<Model> Objects { get; set; }

        public List<Grid<Color>> Animate(long ticks, Model model, Vector3? position = null,
            Vector3? rotation = null, Vector3? velocity = null)
        {
            List<Grid<Color>> frames = new List<Grid<Color>>();

            model.Transform.Position.Offset(position.GetValueOrDefault(Vector3.Zero));
            Vector3 pos = Vector3.Zero;
            Vector3 rot = Vector3.Zero;

            pos.Offset(position.GetValueOrDefault(Vector3.Zero));

            for (long t = 0; t < ticks; t++)
            {
                rot.Offset(rotation.GetValueOrDefault(Vector3.Zero));
                pos.Offset(velocity.GetValueOrDefault(Vector3.Zero));
                rot.Wrap(360.0f);

                frames.Add(Render(model.Mesh, pos, rot, t));
            }

            return frames;
        }

        public Grid<Color> Render(Mesh mesh, Transform transform, long tick = 0)
            => Render(new Model(mesh, transform), tick);

        public Grid<Color> Render(Mesh mesh, Vector3 position, Vector3 rotation, long tick = 0)
            => Render(new Model(mesh, new Transform(position, rotation)), tick);

        public Grid<Color> Render(Model model, long tick = 0)
        {
            GammaPen pen = new GammaPen(Camera.Palette[Gamma.Max]);
            // Console.WriteLine($"Tick {tick}:\nRotation: {model.Transform.Rotation.ToString()} Position: {model.Transform.Position.ToString()}");
            return Rasterizer.Render(model, Camera, pen);
        }
    }
}
