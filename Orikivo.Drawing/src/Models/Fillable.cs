using System.Drawing;

namespace Orikivo.Drawing
{
    public class Fillable
    {
        public GammaColor Background { get; set; }
        public GammaColor Foreground { get; set; }
        public Size Size { get; set; }
        public float Progress { get; set; }
        public AngleF Angle { get; set; }
    }
}
