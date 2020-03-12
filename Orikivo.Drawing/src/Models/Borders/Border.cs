namespace Orikivo.Drawing
{
    /// <summary>
    /// Represents the data of a line that surrounds an image.
    /// </summary>
    public class Border
    {
        public BorderEdge Edge { get; set; } = BorderEdge.Outside;
        public System.Drawing.Color Color { get; set; }
        public int Width { get; set; }
    }
}
