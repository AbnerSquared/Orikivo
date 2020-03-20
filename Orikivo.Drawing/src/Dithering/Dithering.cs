using System;
using System.Drawing;

namespace Orikivo.Drawing
{
    public class Dithering
    {
        public Func<Color, Color> Algorithm { get; private set; }
    }
}
