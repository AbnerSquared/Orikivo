using Discord;
using Orikivo.Drawing;
using Direction = Orikivo.Drawing.Direction;

namespace Arcadia.Graphics
{
    public class FillInfo
    {
        public Color? OutlineColor { get; set; }

        public Gamma Primary { get; set; } = Gamma.Max;

        public Gamma? Secondary { get; set; }

        public GammaPalette Palette { get; set; } = GammaPalette.Default;

        public FillMode Usage { get; set; } = FillMode.None;

        public Direction Direction { get; set; } = Direction.Right;

        // This is only used in bars
        public float? FillPercent { get; set; }
    }
}
