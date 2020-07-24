namespace Arcadia.Graphics
{
    public class ComponentPalette
    {
        public PaletteType Primary { get; set; }
        public PaletteType? Secondary { get; set; }

        public PaletteMixType Mix { get; set; }

        public bool Reverse { get; set; }
    }
}