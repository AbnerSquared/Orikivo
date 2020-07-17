using Newtonsoft.Json;
using System;

namespace Orikivo.Drawing
{
    public struct Coordinate
    {
        public static readonly Coordinate Empty = new Coordinate();

        public static Coordinate Add(Coordinate c, Unit u)
            => new Coordinate(c.X + u.Width, c.Y + u.Height);

        public static Coordinate Subtract(Coordinate c, Unit u)
            => new Coordinate(c.X - u.Width, c.Y - u.Height);

        public static Coordinate Floor(CoordinateF c)
            => new Coordinate((int)MathF.Floor(c.X), (int)MathF.Floor(c.Y));

        [JsonConstructor]
        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Coordinate(int xy)
        {
            X = Y = xy;
        }

        [JsonProperty("x")]
        public int X { get; set; }

        [JsonProperty("y")]
        public int Y { get; set; }

        [JsonIgnore]
        public bool IsEmpty => X == 0 && Y == 0;

        public static implicit operator System.Drawing.Point(Coordinate c)
            => new System.Drawing.Point(c.X, c.Y);

        public static implicit operator Coordinate(System.Drawing.Point p)
            => new Coordinate(p.X, p.Y);
    }
}
