using System.Text;
using static System.MathF;
using Orikivo.Drawing.Graphics3D;

namespace Orikivo.Drawing
{
    // TODO: Replace MatrixF with Grid.
    public struct MatrixF
    {
        public static readonly MatrixF M4x4 = new MatrixF(4, 4);

        public static MatrixF CreateRotation2D(AngleF angle)
        {
            MatrixF m = new MatrixF(2, 2);
            float radians = angle.Radians;

            m[0, 0] = Cos(radians);
            m[0, 1] = -Sin(radians);
            m[1, 0] = Sin(radians);
            m[1, 1] = Cos(radians);

            return m;
        }

        public static MatrixF CreateRotationX(AngleF angle)
        {
            MatrixF m = M4x4;
            float radians = angle.Radians;

            m[0, 0] = 1.0f;
            m[1, 1] = Cos(radians);
            m[1, 2] = Sin(radians);
            m[2, 1] = -Sin(radians);
            m[2, 2] = Cos(radians);
            m[3, 3] = 1.0f;

            return m;
        }

        public static MatrixF CreateRotationY(AngleF angle)
        {
            MatrixF m = M4x4;
            float radians = angle.Radians;

            m[0, 0] = Cos(radians);
            m[0, 2] = Sin(radians);
            m[1, 1] = 1.0f;
            m[2, 0] = -Sin(radians);
            m[2, 2] = Cos(radians);
            m[3, 3] = 1.0f;

            return m;

        }

        public static MatrixF CreateRotationZ(AngleF angle)
        {
            MatrixF m = M4x4;
            float radians = angle.Radians;

            m[0, 0] = Cos(radians);
            m[0, 1] = Sin(radians);
            m[1, 0] = -Sin(radians);
            m[1, 1] = Cos(radians);
            m[2, 2] = 1.0f;
            m[3, 3] = 1.0f;

            return m;
        }

        public static MatrixF CreateProjector(float near, float far, float fov, float aspectRatio)
        {
            MatrixF m = M4x4;

            float fovRad = 1.0f / Tan(Utils.Radians(fov / 2.0f));
            //float normal = far / (far - near);

            m[0, 0] = aspectRatio * fovRad;
            m[1, 1] = fovRad;
            m[2, 2] = far / (far - near);
            m[3, 2] = (-far * near) / (far - near);
            m[2, 3] = 1.0f;
            m[3, 3] = 0.0f;

            return m;
        }

        public static MatrixF FromVector(Vector3 vector)
        {
            MatrixF m = M4x4;
            m[0, 0] = vector.X;
            m[1, 1] = vector.Y;
            m[2, 2] = vector.Z;
            m[3, 3] = 1.0f;

            return m;
        }

        public MatrixF(int width, int height)
        {
            Values = new float[height, width];
        }

        public float[,] Values { get; }

        public int Width => Values.GetLength(1);

        public int Height => Values.GetLength(0);

        public void SetValue(float value, int row, int column)
            => Values[row, column] = value;

        public float[] GetRow(int y)
        {
            float[] row = new float[Width];

            for (int x = 0; x < Width; x++)
                row[x] = Values[y, x];

            return row;
        }

        public float[] GetColumn(int x)
        {
            float[] column = new float[Height];

            for (int y = 0; y < Height; y++)
                column[y] = Values[y, x];

            return column;
        }

        public float this[int row, int column]
        {
            get => Values[row, column];
            set => SetValue(value, row, column);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int r = 0; r < Height; r++)
                sb.AppendLine(string.Join(" ", GetRow(r)));

            return sb.ToString();
        }
    }
}
