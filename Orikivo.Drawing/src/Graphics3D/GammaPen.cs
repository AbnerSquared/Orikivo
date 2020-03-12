namespace Orikivo.Drawing.Graphics3D
{
    public class GammaPen
    {
        public GammaPen(GammaColor color)
        {
            Color = color;
            IsDown = false;
        }

        // X, Y: Go to X, Y

        public bool IsDown { get; set; }

        public GammaColor Color { get; set; }
    }
}
