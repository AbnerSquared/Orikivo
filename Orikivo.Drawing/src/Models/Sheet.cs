using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Orikivo.Drawing
{
    /// <summary>
    /// Represents a <see cref="Sprite"/> that contains a collection of images according to a grid-based pattern.
    /// </summary>
    public sealed class Sheet : Sprite
    {
        [JsonConstructor]
        public Sheet(string url, int cropWidth, int cropHeight, List<SheetOverride> overrides = null, string id = null)
            : base(url, id)
        {
            Overrides = overrides ?? new List<SheetOverride>();
            CropHeight = cropHeight;
            CropWidth = cropWidth;

            using Bitmap source = Load();

            if (source.Width % CropWidth != 0)
                throw new ArgumentException("The crop width specified leaves a remainder width.", nameof(cropWidth));

            if (source.Height % CropHeight != 0)
                throw new ArgumentException("The crop height specified leaves a remainder height.", nameof(cropHeight));

            ColumnCount = source.Width / CropWidth;
            RowCount = source.Height / CropHeight;
        }

        /// <summary>
        /// A collection of custom specifications for a specific crop.
        /// </summary>
        [JsonProperty("overrides")]
        private List<SheetOverride> Overrides { get; }

        /// <summary>
        /// Represents the image cropping width of this <see cref="Sheet"/>.
        /// </summary>
        [JsonProperty("crop_width")]
        public int CropWidth { get; }

        /// <summary>
        /// Represents the image cropping height of this <see cref="Sheet"/>.
        /// </summary>
        [JsonProperty("crop_height")]
        public int CropHeight { get; }

        /// <summary>
        /// Represents the total number of rows on this <see cref="Sheet"/>.
        /// </summary>
        [JsonIgnore]
        public int RowCount { get; }

        /// <summary>
        /// Represents the total number of columns on this <see cref="Sheet"/>.
        /// </summary>
        [JsonIgnore]
        public int ColumnCount { get; }

        /// <summary>
        /// Returns the cropped image at the specified index.
        /// </summary>
        public Bitmap GetSprite(int index)
        {
            if (index > ColumnCount * RowCount || index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "The specified index is out of range");

            int column = 1;

            while (index - ColumnCount > 0)
            {
                column++;
                index -= ColumnCount;
            }

            return GetSprite(index, column);
        }

        /// <summary>
        /// Returns the cropped image at the specified row and column.
        /// </summary>
        public Bitmap GetSprite(int row, int column)
        {
            if (row > RowCount)
                throw new ArgumentOutOfRangeException(nameof(row), "The specified row index is out of range.");

            if (column > ColumnCount)
                throw new ArgumentOutOfRangeException(nameof(column), "The specified column index is out of range.");

            SheetOverride crop = Overrides.FirstOrDefault(x => x.Row == row && x.Column == column) ?? SheetOverride.Empty;

            if (RowCount == 1 && ColumnCount == 1)
                return Load();

            return ImageHelper.Crop(Path,
                (CropWidth * (column - 1)) + crop.OffsetX,
                (CropHeight * (row - 1)) + crop.OffsetY,
                crop.Width ?? CropWidth,
                crop.Height ?? CropHeight);
        }

        /// <summary>
        /// Returns all of the image crops with their corresponding row and column.
        /// </summary>
        public List<(Coordinate Position, Bitmap Bitmap)> GetSprites()
        {
            var bitmaps = new List<(Coordinate, Bitmap)>();

            for (int x = 1; x <= ColumnCount; x++)
            {
                for (int y = 1; y <= RowCount; y++)
                {
                    bitmaps.Add((new Point(y, x), GetSprite(y, x)));
                }
            }

            return bitmaps;
        }
    }
}
