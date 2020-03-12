using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Orikivo.Drawing
{
    public abstract class LayerGroup : DrawableLayer
    {
        public List<DrawableLayer> Layers { get; } = new List<DrawableLayer>();
        public List<DrawableLayer> UngroupLayers() // what this class does, is modify the offsets made here to each layer synchronously.
        {
            // this makes it easier to modify layers at the same time.
            // each layer in this class can have their own local offset for each other, which is utilized

            throw new NotImplementedException();
        }
    }
    public abstract class LayerGroup<TLayer> : DrawableLayer
        where TLayer : DrawableLayer
    {

    }


    public abstract class DrawableLayer : IDisposable
    {
        protected bool Disposed { get; set; }

        /// <summary>
        /// The max width the <see cref="DrawableLayer"/> can be set to.
        /// </summary>
        public int? MaxWidth { get; set; }

        /// <summary>
        /// The max height the <see cref="DrawableLayer"/> can be set to.
        /// </summary>
        public int? MaxHeight { get; set; }

        /// <summary>
        /// The offset that is applied to the <see cref="Drawable.Origin"/>.
        /// </summary>
        public Point Offset { get; set; }

        /// <summary>
        /// The extraneous whitespace that will surround the <see cref="DrawableLayer"/>.
        /// </summary>
        public Padding Padding { get; set; }

        /// <summary>
        /// The literal <see cref="Point"/> of where the <see cref="Bitmap"/> is drawn.
        /// </summary>
        public Point Position => new Point(Offset.X + Padding.Left, Offset.Y + Padding.Top);

        public DrawableConfig Config { get; set; }

        /// <summary>
        /// Returns the <see cref="Bitmap"/> initialized from the <see cref="DrawableLayer"/>.
        /// </summary>
        protected abstract Bitmap GetBaseImage();

        /*
        /// <summary>
        /// Returns the <see cref="Size"/> of the <see cref="Bitmap"/> returned from <see cref="GetBaseImage"/>.
        /// </summary>
        public Size GetSize()
        {
            using (Bitmap bmp = GetBaseImage())
            {
                return new Size(MaxWidth.HasValue ? bmp.Width > MaxWidth.Value ? MaxWidth.Value : bmp.Width : bmp.Width,
                    MaxHeight.HasValue ? bmp.Height > MaxHeight.Value ? MaxHeight.Value : bmp.Height : bmp.Height);
            }
        }
        */

        /// <summary>
        /// Returns the true <see cref="Bitmap"/> from the <see cref="DrawableLayer"/>.
        /// </summary>
        /// <returns></returns>
        public virtual Bitmap Build()
        {
            if (Disposed)
                throw new ObjectDisposedException("The layer has already been disposed.");

            using (Bitmap image = GetBaseImage())
            {
                Size size = new Size(
                    MaxWidth.HasValue ? image.Width > MaxWidth.Value ? MaxWidth.Value : image.Width : image.Width,
                    MaxHeight.HasValue ? image.Height > MaxHeight.Value ? MaxHeight.Value : image.Height : image.Height);

                Bitmap result = new Bitmap(size.Width + Padding.Width, size.Height + Padding.Height);

                // TODO: Use PixelGraphics
                using (Graphics graphics = Graphics.FromImage(result))
                {
                    // TODO: Make this method generic for any image.
                    GraphicsUtils.ClipAndDrawImage(graphics, image, new Point(Padding.Left, Padding.Top));


                    // TODO: Implement ImageConfig manipulation.
                }


                return result;
            }
        }

        /// <summary>
        /// Disposes any possible <see cref="Bitmap"/> remainders.
        /// </summary>
        public virtual void Dispose() { }
    }
}
