using System;
using Orikivo.Drawing.Graphics3D;

namespace Orikivo.Drawing
{
    public static class GridExtensions
    {
        /// <summary>
        /// Sets the value of a grid coordinate by a specified <see cref="System.Drawing.Point"/>.
        /// </summary>
        public static void SetValue<T>(this Grid<T> grid, T value, System.Drawing.Point p)
            => grid.SetValue(value, p.X, p.Y);
        public static T GetValue<T>(this Grid<T> grid, System.Drawing.Point p)
             => grid.GetValue(p.X, p.Y);

        public static Grid<T> GetRegion<T>(this Grid<T> grid, System.Drawing.Rectangle rectangle)
            => grid.GetRegion(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

        public static Grid<T> GetRegion<T>(this Grid<T> grid, System.Drawing.Point point, System.Drawing.Size size)
            => grid.GetRegion(point.X, point.Y, size.Width, size.Height);

        public static Grid<T> GetPartialRegion<T>(this Grid<T> grid, System.Drawing.Point point, System.Drawing.Size size)
            => grid.GetPartialRegion(point.X, point.Y, size.Width, size.Height);

        public static Grid<Vector3> Offset(this Grid<Vector3> grid, Vector3 v)
        {
            throw new NotImplementedException();
        }

        public static Grid<Vector3> Offset(this Grid<Vector3> grid, float x, float y, float z)
        {
            throw new NotImplementedException();
        }

        public static Grid<float> Add(this Grid<float> grid, float f)
        {
            throw new NotImplementedException();
        }

        public static Grid<float> Subtract(this Grid<float> grid, float f)
        {
            throw new NotImplementedException();
        }

        // this multiplies as if they were matrices
        public static Grid<float> Multiply(this Grid<float> grid, Grid<float> matrix)
        {
            throw new NotImplementedException();
        }

        // this multiplies all values on the grid by a specified value
        public static Grid<float> Multiply(this Grid<float> grid, float f)
        {
            throw new NotImplementedException();
        }

        public static Grid<float> Divide(this Grid<float> grid, float f)
        {
            throw new NotImplementedException();
        }

        public static Grid<float> Pow(this Grid<float> grid, float f)
        {
            throw new NotImplementedException();
        }

        public static Grid<float> Sqrt(this Grid<float> grid)
        {
            throw new NotImplementedException();
        }

        // this gets a grid from which the last grid is 
        public static Grid<float> Cbrt(this Grid<float> grid)
        {
            throw new NotImplementedException();
        }

        // this adds up all float values on this grid.
        public static float Sum(this Grid<float> grid)
        {
            float sum = 0;
            grid.ForEachValue((x, y, z) => sum += z);

            return sum; 
        }

        // this sums all of the columns, and returns an array of all summed columns.
        public static float[] SumColumns(this Grid<float> grid)
        {
            throw new NotImplementedException();
        }

        // this sums all of the rows, and returns an array of all summed rows
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

        // adds up all float values on a specified row.
        public static float SumRow(this Grid<float> grid, int rowIndex)
        {
            throw new NotImplementedException();
        }

        public static Grid<float> SwapRow(this Grid<float> grid, int fromIndex, int toIndex)
        {
            throw new NotImplementedException();
        }

        public static Grid<float> Ceiling(this Grid<float> grid)
        {
            return grid.Select(x => MathF.Ceiling(x));
        }

        public static Grid<float> Floor(this Grid<float> grid)
        {
            return grid.Select(x => MathF.Floor(x));
        }

        // removes all decimals
        public static Grid<float> Truncate(this Grid<float> grid)
        {
            throw new NotImplementedException();
        }

        public static Grid<float> Round(this Grid<float> grid)
        {
            throw new NotImplementedException();
        }

        // this transposes the grid specified.
        public static Grid<float> Transpose(this Grid<float> grid)
        {
            throw new NotImplementedException();
        }

        public static Grid<Vector2> Offset(this Grid<Vector2> grid, Vector2 v)
            => Offset(grid, v.X, v.Y);

        public static Grid<Vector2> Offset(this Grid<Vector2> grid, float u, float v)
        {
            grid.ForEachValue((int x, int y, Vector2 z) => z.Offset(u, v));
            return grid;
        }

        public static Grid<T?> GetRegionOrDefault<T>(this Grid<T> grid, int x, int y, int width, int height) where T : struct
        {
            Grid<T?> region = new Grid<T?>(width, height);

            for (int py = 0; py < height; py++)
                for (int px = 0; px < width; px++)
                    if (grid.Contains(px + x, py + y))
                        region.SetValue(grid.GetValue(x + px, y + py), px, py);

            return region;
        }
    }
}
