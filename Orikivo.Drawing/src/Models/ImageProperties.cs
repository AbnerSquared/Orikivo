using System.Drawing;

namespace Orikivo.Drawing
{
    public class ImageProperties
    {
        // background color
        public Color? Matte { get; set; }

        public float Opacity { get; set; }

        public int? Width { get; set; }
        
        public int? Height { get; set; }

        public Padding Padding { get; set; }

        // if true, text rows will expand to include offsets
        public bool ExpandRowOnOffset { get; set; }

        public bool TrimEmptyPixels { get; set; }
    }
}
