using Orikivo.Drawing;

namespace Arcadia.Graphics
{
    public class BorderInfo
    {
        public BorderAllow Allowed { get; set; }

        public BorderEdge Edge { get; set; } = BorderEdge.Outside;

        public FillInfo Fill { get; set; }

        public int Thickness { get; set; }
    }
}
