namespace Orikivo.Drawing.Graphics2D
{
    public class Pen
    {
        // determines what color this pen draws at.
        public System.Drawing.Color Color { get; set; }

        // determines the size of the brush to use.
        public float Size { get; set; }

        // determines if the pen is currently active.
        public bool IsDown { get; set; }
    }
}
