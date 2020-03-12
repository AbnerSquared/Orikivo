namespace Orikivo.Drawing.Graphics2D
{
    public class Canvas
    {
        // individual pixels.
        // a grid containing all pixels that can be drawn.
        public Grid<System.Drawing.Color> Pixels { get; set; }


        // the currently active pen.
        public Pen Pen { get; set; }

        public void PenDown() { }

        public void PenUp() { }

        public void Clear() { }
        
        public void Stamp(System.Drawing.Image image)
        { }
    }
}
