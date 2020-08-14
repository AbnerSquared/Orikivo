using Newtonsoft.Json;

namespace Orikivo.Drawing
{
    public struct Padding
    {
        public static readonly Padding Char = new Padding(right: 1);
        public static readonly Padding Empty = new Padding(0);

        public Padding(int lrtb)
        {
            Left = Right = Top = Bottom = lrtb;
        }

        [JsonConstructor]
        public Padding(int left = 0, int right = 0, int top = 0, int bottom = 0)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        [JsonProperty("left")]
        public int Left { get; set; }

        [JsonProperty("right")]
        public int Right { get; set; }

        [JsonProperty("top")]
        public int Top { get; set; }

        [JsonProperty("bottom")]
        public int Bottom { get; set; }

        [JsonIgnore]
        public int Width => Left + Right;

        [JsonIgnore]
        public int Height => Top + Bottom;

        [JsonIgnore]
        public bool IsEmpty =>
            Left == 0
            && Right == 0
            && Bottom == 0
            && Right == 0;

        public Padding Clone()
            => new Padding(Left, Right, Top, Bottom);

        public static implicit operator Padding(int lrtb)
            => new Padding(lrtb);
    }
}
