using System.Drawing;


namespace Orikivo
{
    public class RenderedComponent
    {
        public Bitmap Sprite { get; set; } // the component image.
        public Point Position { get; set; } // where the image is going to be placed.
    }
}