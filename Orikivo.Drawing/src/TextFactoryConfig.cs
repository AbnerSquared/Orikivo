using System.Collections.Generic;

namespace Orikivo.Drawing
{
    // TODO: Implement usage of custom directories
    public class TextFactoryConfig
    {
        public static TextFactoryConfig Default = new TextFactoryConfig
        {
            //FontDirectory = "../assets/fonts/",
            CanCacheChars = true
        };

        public bool CanCacheChars { get; set; }

        public List<FontFace> Fonts { get; set; }
        
        public char[][][][] CharMap { get; set; }
    }
}
