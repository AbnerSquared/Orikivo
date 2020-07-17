namespace Orikivo.Drawing.Graphics3D
{
    public class GammaPen
    {
        public GammaPen(ImmutableColor color)
        {
            Color = color;
            IsDown = false;
        }

        // X, Y: Go to X, Y

        public bool IsDown { get; set; }

        public ImmutableColor Color { get; set; }
    }
}
