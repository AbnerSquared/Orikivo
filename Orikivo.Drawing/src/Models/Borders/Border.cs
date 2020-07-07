namespace Orikivo.Drawing
{
    /// <summary>
    /// Represents the data of a line that surrounds an image.
    /// </summary>
    public class Border
    {
        // this is used to know what sides are drawn with this border.
        public BorderAllow Allow { get; set; } = BorderAllow.All;

        public BorderEdge Edge { get; set; } = BorderEdge.Outside;

        public System.Drawing.Color Color { get; set; }

        public int Thickness { get; set; }
    }
}
