namespace Orikivo.Text
{
    /// <summary>
    /// A raw character with a defined X and Y coordinate value.
    /// </summary>
    public class CharValue
    {
        public CharValue(char c, int x, int y)
        {
            Char = c;
            X = x;
            Y = y;
        }

        /// <summary>
        /// The character value that will be drawn onto the <see cref="CharGrid"/>.
        /// </summary>
        public char Char { get; }

        /// <summary>
        /// The X coordinate (used for the <see cref="CharGrid"/>).
        /// </summary>
        public int X { get; }

        /// <summary>
        /// The Y coordinate (used for the <see cref="CharGrid"/>).
        /// </summary>
        public int Y { get; }
    }
}
