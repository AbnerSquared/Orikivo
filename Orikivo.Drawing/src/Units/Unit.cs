using Newtonsoft.Json;

namespace Orikivo.Drawing
{
    /// <summary>
    /// Represents a JSON-friendly variant of <see cref="System.Drawing.Size"/>.
    /// </summary>
    public struct Unit
    {
        [JsonConstructor]
        public Unit(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public Unit(int wh)
        {
            Width = Height = wh;
        }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        public bool IsEmpty => Width == 0 && Height == 0;

        public static implicit operator System.Drawing.Size(Unit u)
            => new System.Drawing.Size(u.Width, u.Height);

        public static implicit operator Unit(System.Drawing.Size s)
            => new Unit(s.Width, s.Height);
    }
}
