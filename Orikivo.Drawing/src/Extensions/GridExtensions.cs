using System;
using Orikivo.Drawing.Graphics3D;

namespace Orikivo.Drawing
{
    internal static class GridExtensions
    {
        public static void SetValue<T>(this Grid<T> grid, T value, System.Drawing.Point p)
            => grid.SetValue(value, p.X, p.Y);

        public static T GetValue<T>(this Grid<T> grid, System.Drawing.Point p)
             => grid.GetValue(p.X, p.Y);

        public static Grid<T> GetRegion<T>(this Grid<T> grid, System.Drawing.Rectangle rectangle)
            => grid.GetRegion(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

        public static Grid<T> GetRegion<T>(this Grid<T> grid, System.Drawing.Point point, System.Drawing.Size size)
            => grid.GetRegion(point.X, point.Y, size.Width, size.Height);

        public static Grid<T> GetPartialRegion<T>(this Grid<T> grid, System.Drawing.Rectangle rectangle)
            => grid.GetPartialRegion(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

        public static Grid<T> GetPartialRegion<T>(this Grid<T> grid, System.Drawing.Point point, System.Drawing.Size size)
            => grid.GetPartialRegion(point.X, point.Y, size.Width, size.Height);

        public static Grid<T?> GetRegionOrDefault<T>(this Grid<T> grid, int x, int y, int width, int height)
            where T : struct
        {
            Grid<T?> region = new Grid<T?>(width, height);

            for (int py = 0; py < height; py++)
                for (int px = 0; px < width; px++)
                    if (grid.Contains(px + x, py + y))
                        region.SetValue(grid.GetValue(x + px, y + py), px, py);

            return region;
        }

        public static void Offset(this Grid<Vector2> grid, Vector2 v)
            => grid.ForEachValue(p => p.Offset(v));

        public static void Offset(this Grid<Vector2> grid, float u, float v)
            => grid.ForEachValue(p => p.Offset(u, v));

        public static void Offset(this Grid<Vector3> grid, Vector3 v)
            => grid.ForEachValue(p => p.Offset(v));

        public static void Offset(this Grid<Vector3> grid, float u, float v, float w)
            => grid.ForEachValue(p => p.Offset(u, v, w));

        public static Grid<float> Add(this Grid<float> grid, float f)
            => grid.Select(z => z + f);

        public static Grid<float> Multiply(this Grid<float> grid, float f)
            => grid.Select(z => z * f);

        public static Grid<float> Divide(this Grid<float> grid, float f)
            => grid.Select(z => z / f);

        public static Grid<float> Pow(this Grid<float> grid, float f)
            => grid.Select(z => MathF.Pow(z, f));

        public static Grid<float> Sqrt(this Grid<float> grid)
            => grid.Select(z => MathF.Sqrt(z));

        public static Grid<float> Cbrt(this Grid<float> grid)
            => grid.Select(z => MathF.Cbrt(z));

        public static Grid<float> Ceiling(this Grid<float> grid)
            => grid.Select(x => MathF.Ceiling(x));

        public static Grid<float> Floor(this Grid<float> grid)
            => grid.Select(x => MathF.Floor(x));

        public static Grid<float> Truncate(this Grid<float> grid)
            => grid.Select(z => MathF.Truncate(z));

        public static Grid<float> Round(this Grid<float> grid)
            => grid.Select(z => MathF.Round(z));

        public static float Sum(this Grid<float> grid)
        {
            float sum = 0;
            grid.ForEachValue((x, y, z) => sum += z);

            return sum; 
        }

        public static float[] SumColumns(this Grid<float> grid)
        {
            var colSums = new float[grid.Width];
            int i = 0;

            grid.ForEachColumn(delegate(float[] col)
            {
                float sum = 0;

                for (int c = 0; c < col.Length; c++)
                {
                    sum += col[c];
                }

                colSums[i] = sum;
                i++;
            });

            return colSums;
        }

        public static float[] SumRows(this Grid<float> grid)
        {
            var rowSums = new float[grid.Height];
            int i = 0;

            grid.ForEachRow(delegate (float[] row)
            {
                float sum = 0;

                for (int r = 0; r < row.Length; r++)
                {
                    sum += row[r];
                }

                rowSums[i] = sum;
                i++;
            });

            return rowSums;
        }

        public static Grid<float> Transpose(this Grid<float> grid)
        {
            var t = new Grid<float>(grid.Height, grid.Width);

            int i = 0;
            grid.ForEachRow(delegate (float[] row)
            {
                t.SetColumn(i, row);
                i++;
            });

            return t;
        }

        // TODO: Implement methods written below
        /*
        public static Grid<float> Multiply(this Grid<float> grid, Grid<float> matrix)
        {
            throw new NotImplementedException();
        } 
         
        public static float SumRow(this Grid<float> grid, int rowIndex)
        {
            throw new NotImplementedException();
        }

        public static Grid<float> SwapRow(this Grid<float> grid, int fromIndex, int toIndex)
        {
            throw new NotImplementedException();
        }
        */
    }
}
