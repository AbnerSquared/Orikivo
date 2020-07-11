using System.Drawing;

namespace Orikivo.Drawing
{
    public class CanvasOptions // TODO: use ImageProperties instead.
    {
        public CanvasOptions() { }

        // determines if the row's padding underneath is stretched to contain the offset.
        public Color? BackgroundColor { get; set; }
        public bool? UseNonEmptyWidth { get; set; }
        public bool ExtendOnOffset { get; set; }
        public int? Width { get; set; } // if empty, is automatically set based on the action.
        public int? Height { get; set; }
        public Padding Padding { get; set; }
    }
}
