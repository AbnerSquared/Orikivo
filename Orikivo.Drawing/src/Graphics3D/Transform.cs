namespace Orikivo.Drawing.Graphics3D
{
    public class Transform
    {
        public static Transform Default = new Transform(Vector3.Zero, Vector3.Zero, Vector3.One);

        public Transform(Vector3 position, Vector3 rotation, Vector3? scale = null)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale.GetValueOrDefault(Vector3.One);
        }

        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }
    }
}
