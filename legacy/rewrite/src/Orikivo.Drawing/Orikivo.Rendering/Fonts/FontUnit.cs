using Newtonsoft.Json;

namespace Orikivo
{
    /// <summary>
    /// Represents a grid map scale of a font face.
    /// </summary>
    public class FontUnit
    {
        public FontUnit()
        {

        }

        /// <summary>
        /// Constructs a FontUnit with one integer representing both sides.
        /// </summary>
        public FontUnit(int wh)
        {
            Width = wh;
            Height = wh;
        }

        /// <summary>
        /// Constructs a FontUnit using both a specified width and height.
        /// </summary>
        [JsonConstructor]
        public FontUnit(int width, int height)
        {
            Width = width;
            Height = height;
        }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        public override string ToString()
            => $"({Width}px, {Height}px)";
    }
}