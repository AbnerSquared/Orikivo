using Discord;
using Orikivo.Drawing;
using Direction = Orikivo.Drawing.Direction;

namespace Arcadia.Graphics
{
    public class FillInfo
    {
        public Color? OutlineColor;
        public Gamma? Primary;
        public Gamma? Secondary;
        public GammaPalette Palette;
        public FillMode Usage;
        public Direction Direction;
    }
}