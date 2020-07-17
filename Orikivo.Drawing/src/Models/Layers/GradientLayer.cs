using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Orikivo.Drawing
{
    public class GradientLayer : DrawableLayer
    {
        public Dictionary<float, Color> Markers { get; set; } = new Dictionary<float, Color>();

        public GradientColorHandling ColorHandling { get; set; } = GradientColorHandling.Blend;
        public int Width { get; set; }
        public int Height { get; set; }
        public AngleF Angle { get; set; } = AngleF.Right;

        protected override Bitmap GetBaseImage()
            => ImageEditor.CreateGradient(Markers, Width, Height, ColorHandling);
    }
}
