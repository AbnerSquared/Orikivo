using Newtonsoft.Json;
using System.Drawing;
using System.IO;

namespace Orikivo.Drawing
{
    // TODO: Implement Sprite.FileExtension, which is the file extension.
    /// <summary>
    /// Represents an image reference from a local path.
    /// </summary>
    public class Sprite
    {
        [JsonConstructor]
        public Sprite(string url, string id = null)
        {
            Id = id;
            Path = url;

            if (!File.Exists(Path))
                throw new System.Exception("The specified path does not point to an existing file.");

            using (Bitmap source = GetImage())
            {
                Width = source.Width;
                Height = source.Height;
            }
        }

        /// <summary>
        /// Represents a unique identifier for this <see cref="Sprite"/>.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// A local path that points to the <see cref="Image"/> referenced for this <see cref="Sprite"/>.
        /// </summary>
        [JsonProperty("path")]
        public string Path { get; }

        /// <summary>
        /// Represents the width of the source for this <see cref="Sprite"/>.
        /// </summary>
        [JsonIgnore]
        public int Width { get; }

        /// <summary>
        /// Represents the height of the source for this <see cref="Sprite"/>.
        /// </summary>
        [JsonIgnore]
        public int Height { get; }

        /// <summary>
        /// Loads the <see cref="Bitmap"/> referenced from this <see cref="Sprite"/>.
        /// </summary>
        public Bitmap GetImage()
            => new Bitmap(Path);
    }
}
