using System.Drawing;

namespace Orikivo.Drawing
{
    public struct Cursor
    {
        public Cursor(int x = 0, int y = 0)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public void Set(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Move(int x, int y)
        {
            X += x;
            Y += y;
        }

        public static implicit operator Point(Cursor cursor)
            => new Point(cursor.X, cursor.Y);
    }
}
