﻿using System.Drawing;

namespace Orikivo.Drawing
{
    public class FillableLayer : DrawableLayer
    {
        public GammaColor Background { get; set; }
        public GammaColor Foreground { get; set; }
        public Size Size { get; set; }
        public float Progress { get; set; }
        public AngleF Angle { get; set; }

        protected override Bitmap GetBaseImage()
        {
            throw new System.NotImplementedException();
        }
    }
}
