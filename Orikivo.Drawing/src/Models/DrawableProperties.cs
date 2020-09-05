namespace Orikivo.Drawing
{
    public class DrawableProperties
    {
        public static readonly DrawableProperties Default = new DrawableProperties
        {
            Padding = Padding.Empty,
            Margin = Padding.Empty,
            Shadow = null,
            Scale = Vector2.One,
            Matte = null,
            Palette = GammaPalette.Default,
            ColorHandling = DrawablePaletteHandling.Map,
            Opacity = 1,
            Border = null
        };

        public DrawablePaletteHandling ColorHandling { get; set; } = DrawablePaletteHandling.Map;

        public GammaPalette Palette { get; set; }
        
        public Gamma? Matte { get; set; }
        
        public float Opacity { get; set; }

        public Vector2 Scale { get; set; }
        
        public Padding Padding { get; set; } // BEFORE Border

        public Padding Margin { get; set; } // AFTER Border

        public Shadow Shadow { get; set; }

        public Border Border { get; set; }

        public DrawableProperties WithScale(ImageScale scale)
        {
            Scale = new Vector2((int)scale, (int)scale);
            return this;
        }
    }
}
