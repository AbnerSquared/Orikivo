namespace Orikivo.Drawing
{
    // TODO: Replace generic thickness with Padding class to allow custom thicknesses on any edges
    //       This can also replace the BorderAllow enum, since marking an edge with thickness 0 is the same as leaving it blank
    public class Border
    {
        public BorderAllow Allow { get; set; } = BorderAllow.All;

        public BorderEdge Edge { get; set; } = BorderEdge.Outside;

        public Padding lrtb { get; set; }

        public System.Drawing.Color Color { get; set; }

        public int Thickness { get; set; }
    }
}
