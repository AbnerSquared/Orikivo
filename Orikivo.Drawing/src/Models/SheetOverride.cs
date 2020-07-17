using Newtonsoft.Json;

namespace Orikivo.Drawing
{
    /// <summary>
    /// Represents a custom size and origin for a <see cref="Sprite"/> within a <see cref="Sheet"/>.
    /// </summary>
    public class SheetOverride
    {
        public static readonly SheetOverride Empty = new SheetOverride(0, 0);

        [JsonConstructor]
        public SheetOverride(int row,
            int column,
            int? offsetX = null,
            int? offsetY = null,
            int? width = null,
            int? height = null)
        {
            Row = row;
            Column = column;
            OffsetX = offsetX.GetValueOrDefault(0);
            OffsetY = offsetY.GetValueOrDefault(0);
            Width = width;
            Height = height;
        }

        [JsonProperty("row")]
        public int Row { get; }

        [JsonProperty("col")]
        public int Column { get; }

        [JsonProperty("offset_x")]
        public int OffsetX { get; }

        [JsonProperty("offset_y")]
        public int OffsetY { get; }

        [JsonProperty("width")]
        public int? Width { get; }

        [JsonProperty("height")]
        public int? Height { get; }
    }
}
