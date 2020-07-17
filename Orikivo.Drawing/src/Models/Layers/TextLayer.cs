using System.Drawing;

namespace Orikivo.Drawing
{
    // TODO: Rework and organize
    public class TextLayer : DrawableLayer
    {
        public FontFace Font { get; set; }

        public string Text { get; set; }

        public char[][][][] CharMap { get; set; }

        public ImmutableColor Color { get; set; }

        protected override Bitmap GetBaseImage()
        {
            TextFactoryConfig config = TextFactoryConfig.Default;
            config.CharMap = CharMap;
            config.Fonts.Add(Font);

            using (TextFactory writer = new TextFactory(config))
            {
                writer.SetFont(Font);
                return writer.DrawText(Text, Color);
            }
        }
    }
}
