using System.Collections.Generic;

namespace Orikivo.Drawing
{
    public class GraphicsConfig
    {
        public static GraphicsConfig Default = new GraphicsConfig
        {
            // TODO: Handle CharMap deserialization with JsonCharArrayConverter
            // TODO: Allow importing of fonts automatically from a directory
            FontDirectory = "../assets/fonts/",
            Palette = GammaPalette.Default,
            CacheChars = true
        };

        public char[][][][] CharMap { get; set; } // Create default char map
        public List<FontFace> Fonts { get; set; } = new List<FontFace>();
        public GammaPalette Palette { get; set; } = GammaPalette.Default;
        public string FontDirectory { get; set; }
        public bool CacheChars { get; set; }
    }
}
