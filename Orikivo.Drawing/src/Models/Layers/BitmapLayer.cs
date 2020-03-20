using System.Drawing;

namespace Orikivo.Drawing
{
    public class BitmapLayer : DrawableLayer
    {
        public Bitmap Source { get; set; }

        protected override Bitmap GetBaseImage()
            => Source;

        public override void Dispose()
        {
            if (!Disposed)
            {
                Source.Dispose();
                Disposed = true;
            }
        }
    }
}
