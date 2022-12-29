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
            ColorHandling = DrawableColorMode.Map,
            Opacity = 1,
            Border = null
        };

        public DrawableColorMode ColorHandling { get; set; } = DrawableColorMode.Map;

        public GammaPalette Palette { get; set; }

        public Gamma? Matte { get; set; }

        public float Opacity { get; set; }

        public Vector2 Scale { get; set; }

        public Padding Padding { get; set; }

        public Padding Margin { get; set; }

        public Shadow Shadow { get; set; }

        public Border Border { get; set; }
    }
}
