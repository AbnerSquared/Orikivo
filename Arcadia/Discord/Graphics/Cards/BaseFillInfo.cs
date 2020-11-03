using Orikivo.Drawing;

namespace Arcadia.Graphics
{
    public class BaseFillInfo
    {
        public FillMode Mode { get; set; } = FillMode.None;

        public Gamma? Primary { get; set; }

        public Gamma? Secondary { get; set; }

        public Direction Direction { get; set; }

        public float? FillPercent { get; set; }
    }
}
