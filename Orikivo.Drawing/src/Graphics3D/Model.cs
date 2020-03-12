namespace Orikivo.Drawing.Graphics3D
{
    public class Model
    {
        public Model(Mesh mesh, Transform transform = null)
        {
            Mesh = mesh;
            transform ??= Transform.Default;
            Transform = transform;
        }

        public Mesh Mesh { get; }
        public Transform Transform { get; set; }
    }
}
