using System.Drawing;

namespace Orikivo.Drawing
{
    public class CachedChar
    {
        // the id of the font face that this is cached for.
        public string Id { get; set; }

        // the image value for the char
        public Bitmap Value { get; set; }
    }
}
