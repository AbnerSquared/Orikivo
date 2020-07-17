using Newtonsoft.Json;

namespace Orikivo.Drawing
{
    public struct CoordinateF
    {
        public CoordinateF(float xy)
        {
            X = Y = xy;
        }

        [JsonConstructor]
        public CoordinateF(float x, float y)
        {
            X = x;
            Y = y;
        }

        [JsonProperty("x")]
        public float X { get; set; }

        [JsonProperty("y")]
        public float Y { get; set; }

        [JsonIgnore]
        public bool IsEmpty => X == 0 && Y == 0;

        public static implicit operator System.Drawing.PointF(CoordinateF c)
            => new System.Drawing.PointF(c.X, c.Y);

        public static implicit operator CoordinateF(System.Drawing.PointF p)
            => new CoordinateF(p.X, p.Y);
    }
}
