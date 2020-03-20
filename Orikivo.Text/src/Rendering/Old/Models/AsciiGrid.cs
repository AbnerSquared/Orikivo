using System;
using System.Collections.Generic;
using System.Drawing;
using Padding = Orikivo.Drawing.Padding;

namespace Orikivo.Text
{
    /// <summary>
    /// The main scene at which <see cref="AsciiObject"/> entities interact in.
    /// </summary>
    public class AsciiGrid
    {
        public AsciiGrid(int width, int height, char emptyChar = ' ')
        {
            Width = width;
            Height = height;
            EmptyChar = emptyChar;
            Objects = new List<AsciiObject>();
        }

        /// <summary>
        /// The width of the <see cref="AsciiGrid"/> (in characters).
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// The height of the <see cref="AsciiGrid"/> (in characters).
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// The character that will be used when there is no <see cref="AsciiObject"/> at the specified location.
        /// </summary>
        public char EmptyChar { get; }

        /// <summary>
        /// The current position of time as reference for drawing <see cref="AsciiObject"/> positions.
        /// </summary>
		public float Time { get; private set; } // This is utilized when consistently drawing new frames (might need a separate variant for that).

        /// <summary>
        /// A collection of <see cref="AsciiObject"/> entities that exist within the <see cref="AsciiGrid"/>.
        /// </summary>
		public List<AsciiObject> Objects { get; }

        public void CreateAndAddObject(char[][] chars, int x, int y, int z = 0, GridCollideMethod collideMethod = GridCollideMethod.Ignore, AsciiVector vector = null)
        {
            Objects.Add(new AsciiObject(chars, EmptyChar, x, y, z, collideMethod, vector));
        }

        public void CreateAndAddObject(string value, char separatorChar, int x, int y, int z = 0, GridCollideMethod collideMethod = GridCollideMethod.Ignore, AsciiVector vector = null)
        {
            Objects.Add(new AsciiObject(value, separatorChar, EmptyChar, x, y, z, collideMethod, vector));
        }

        /// <summary>
        /// Returns a string containing the current frame of the <see cref="AsciiGrid"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => ToString(Time);

        /// <summary>
        /// Returns a string containing the frame of the <see cref="AsciiGrid"/> at the specified time.
        /// </summary>
        public string ToString(float time)
        {
            CharGrid grid = new CharGrid(Width, Height, EmptyChar);

            foreach(AsciiObject obj in Objects)
            {
                //Point pos = Utils.GetPos(obj, Width, Height, obj.Collider.GridCollider, Padding.Empty, time);
                //Console.WriteLine($"ToDraw.Position: X: {pos.X} Y: {pos.Y}");
                //grid.DrawObject(obj, pos);

                foreach (CharValue value in Utils.GetPoints(obj, this, time))
                {
                    grid.DrawChar(value);
                }
            }
            // Create a char[][] with a height
            // x + width = final x
            return grid.ToString('\n');
        }
    }
}
