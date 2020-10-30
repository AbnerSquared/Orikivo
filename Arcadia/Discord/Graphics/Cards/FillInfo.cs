using Discord;
using Orikivo.Drawing;
using Direction = Orikivo.Drawing.Direction;

namespace Arcadia.Graphics
{
    public class FillInfo
    {
        public Color? OutlineColor;

        public Gamma Primary { get; set; } = Gamma.Max;

        public Gamma? Secondary;

        public GammaPalette Palette { get; set; } = GammaPalette.Default;

        public FillMode Usage;

        public Direction Direction;

        // This is only used in bars
        public float? FillPercent;
    }
}
