using System.Drawing;

namespace Orikivo.Drawing
{
    // TODO: Rework and organize.
    public class TextLayer : DrawableLayer
    {
        public FontFace Font { get; set; }

        public string Text { get; set; }

        public char[][][][] CharMap { get; set; }

        public GammaColor Color { get; set; }

        // TODO: Fix FontWriter.
        protected override Bitmap GetBaseImage()
        {
            GraphicsConfig config = GraphicsConfig.Default;
            config.CharMap = CharMap;
            config.Fonts.Add(Font);
            using (TextFactory writer = new TextFactory())
            {
                writer.SetFont(Font);
                return writer.DrawText(Text, Color);
            }
        }
    }
}
