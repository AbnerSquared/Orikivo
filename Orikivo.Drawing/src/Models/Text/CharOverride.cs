using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;

namespace Orikivo.Drawing
{
    // NOTE: allows you to set the custom width, height, and offset for a group of characters
    public class CharOverride
    {
        [JsonConstructor]
        public CharOverride(IEnumerable<char> chars, int? width = null, int? height = null, int? offsetX = null, int? offsetY = null)
        {
            Chars = chars;
            Width = width;
            Height = height;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }

        [JsonProperty("chars")]
        public IEnumerable<char> Chars { get; }

        [JsonProperty("width")]
        public int? Width { get; }

        [JsonProperty("height")]
        public int? Height { get; }

        [JsonProperty("offset_x")]
        public int? OffsetX { get; }

        [JsonProperty("offset_y")]
        public int? OffsetY { get; }

        public Point? GetOffset()
            => (OffsetX.HasValue || OffsetY.HasValue) ?
            (Point?) new Point(OffsetX.GetValueOrDefault(0), OffsetY.GetValueOrDefault(0))
            : null;
    }
}
