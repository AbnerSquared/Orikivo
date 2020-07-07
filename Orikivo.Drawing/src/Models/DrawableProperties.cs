﻿using System.Drawing;

namespace Orikivo.Drawing
{
    public class DrawableProperties
    {
        public PaletteDrawMethod PaletteMethod { get; set; } = PaletteDrawMethod.Layer;

        public GammaPalette Palette { get; set; }
        
        public Gamma? BackgroundColorIndex { get; set; }
        
        public float Opacity { get; set; }
        
        public Size Size { get; set; }
        
        public Padding Padding { get; set; }
    }
}
