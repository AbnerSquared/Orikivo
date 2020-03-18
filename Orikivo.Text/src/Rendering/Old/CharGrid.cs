using System;
using System.Drawing;
using System.Linq;

namespace Orikivo.Text
{
    /// <summary>
    /// A raw character grid used to store character values at specified positions.
    /// </summary>
    public class CharGrid
    {
        /// <summary>
        /// Creates a new <see cref="CharGrid"/> with the specified width, height, and empty character value.
        /// </summary>
        public CharGrid(int width, int height, char emptyChar)
        {
            Map = new char[height][];

            for (int y = 0; y < height; y++)
            {
                char[] row = new char[width];

                for (int x = 0; x < width; x++)
                    row[x] = emptyChar;

                Map[y] = row;
            }
        }

        /// <summary>
        /// The two-dimensional <see cref="Array"/> containing all of the character values set on the <see cref="CharGrid"/>.
        /// </summary>
        public char[][] Map { get; private set; }

        /// <summary>
        /// The width of the <see cref="Map"/> (in characters).
        /// </summary>
        public int Width => Utils.GetObjectWidth(Map);

        /// <summary>
        /// The height of the <see cref="Map"/> (in characters).
        /// </summary>
        public int Height => Map.GetLength(0);

        /// <summary>
        /// Returns a <see cref="bool"/> defining if the position specified is within the boundaries specified on the <see cref="CharGrid"/>.
        /// </summary>
        public bool OutOfBounds(int x, int y, int width = 0, int height = 0)
            => x + width >= Width || x < 0 || y + height >= Height || y< 0;

        private void CatchOutOfBounds(int x, int y, int width = 0, int height = 0)
        {
            if (OutOfBounds(x, y, width, height))
                throw new ArgumentException("The coordinates specified are out of bounds for the grid.");
        }

        /// <summary>
        /// Places a <see cref="CharValue"/> onto the <see cref="CharGrid"/>.
        /// </summary>
        public void DrawChar(CharValue value)
            => DrawChar(value.Char, value.X, value.Y);

        /// <summary>
        /// Places a <see cref="char"/> onto the <see cref="CharGrid"/> at the specified X and Y coordinates.
        /// </summary>
        public void DrawChar(char c, int x, int y)
        {
            CatchOutOfBounds(x, y);

            Map[y][x] = c;
        }

        /// <summary>
        /// Places a <see cref="string"/> onto the <see cref="CharGrid"/> at the specified X and Y coordinates, separated by an optional separator character.
        /// </summary>
        public void DrawString(string value, int x, int y, char? separatorChar = null)
        {
            string[] rows = separatorChar.HasValue ? value.Split(separatorChar.Value) : new [] { value };
            int width = Utils.GetObjectWidth(rows);
            int height = rows.Length;

            CatchOutOfBounds(x, y, width, height);

            for (int dy = y; dy < height + y; dy++)
            {
                string row = rows[dy];
                for (int dx = 0; dx < width + x; dx++)
                    Map[dy + y][dx + x] = row[dx];
            }
        }

        /// <summary>
        /// Places an <see cref="AsciiObject"/> onto the <see cref="CharGrid"/> at the specified <see cref="Point"/>.
        /// </summary>
        public void DrawObject(AsciiObject obj, Point pos)
        {
            CatchOutOfBounds(pos.X, pos.Y, obj.Width, obj.Height);

            for (int y = pos.Y; y < obj.Height + pos.Y; y++)
                for (int x = pos.X; x < obj.Width + pos.X; x++)
                    Map[y][x] = obj.Chars[y - pos.Y][x - pos.X];
        }
        
        /// <summary>
        /// Converts the <see cref="CharGrid"/> into a string.
        /// </summary>
        public string ToString(char separatorChar)
            => string.Join(separatorChar.ToString(), Map.Select(x => new string(x)));
    }
}
