namespace Orikivo.Drawing.Graphics3D
{
    // a 3D matrix.
    public struct RotationMatrix
    {
        public RotationMatrix(Vector3 rotation)
        {
            X = MatrixF.CreateRotationX(rotation.X);
            Y = MatrixF.CreateRotationY(rotation.Y);
            Z = MatrixF.CreateRotationZ(rotation.Z);
        }

        public MatrixF X { get; }
        public MatrixF Y { get; }
        public MatrixF Z { get; }
    }
}
