using System;

namespace Orikivo.Text
{
    /// <summary>
    /// An object with applicable physics to be placed inside an <see cref="AsciiGrid"/>.
    /// </summary>
    public class AsciiObject
    {
        /// <summary>
        /// Creates a new <see cref="AsciiObject"/> with a specified string value, separator character, empty character, and its initial X and Y values.
        /// </summary>
        public AsciiObject(string value, char separatorChar, char emptyChar, int x, int y, int z = 0, GridCollideMethod collideMethod = GridCollideMethod.Ignore, AsciiVector vector = null)
        {
            Chars = Utils.CreateObject(value, separatorChar, emptyChar);
            X = x;
            Y = y;
            Z = z;
            Collider = new AsciiCollider(Width, Height, collideMethod);
            Vector = vector ?? AsciiVector.Zero;
        }

		public AsciiObject(char[][] chars, char emptyChar, int x, int y, int z = 0, GridCollideMethod collideMethod = GridCollideMethod.Ignore, AsciiVector vector = null)
        {
            Chars = Utils.CreateObject(chars, emptyChar);
            X = x;
            Y = y;
            Collider = new AsciiCollider(Width, Height, collideMethod);
            Vector = vector ?? AsciiVector.Zero;
		}

        /// <summary>
        /// The initial X position of the <see cref="AsciiObject"/> on an <see cref="AsciiGrid"/>.
        /// </summary>
        public int X { get; }

		/// <summary>
        /// The initial Y position of the <see cref="AsciiObject"/> on an <see cref="AsciiGrid"/>.
        /// </summary>
		public int Y { get; }

		/// <summary>
        /// The layer of the <see cref="AsciiObject"/>.
        /// </summary>
		public int Z { get; } // z == layer. if an object is above another, keep their characters; if two objects are on the same layer, proceed to attempt collision.
        
		/// <summary>
        /// A two-dimensional <see cref="Array"/> containing all of the characters to draw onto an <see cref="AsciiGrid"/>.
        /// </summary>
		public char[][] Chars { get; } // Grid<char>

        /// <summary>
        /// The number of characters that are specified within each row in <see cref="Chars"/>.
        /// </summary>
        public int Width => Utils.GetObjectWidth(Chars);

        /// <summary>
        /// The number of rows that are specified in <see cref="Chars"/>.
        /// </summary>
        public int Height => Chars.GetLength(0);

        /// <summary>
        /// The collider that is used to determine collision within the <see cref="AsciiGrid"/> its placed in.
        /// </summary>
        public AsciiCollider Collider { get; }

        /// <summary>
        /// The initial vector for an <see cref="AsciiObject"/>, defining both its velocity and acceleration in the X and Y direction.
        /// </summary>
		public AsciiVector Vector { get; }
    }
}
