using System;
using System.Drawing;

namespace Orikivo.Drawing
{
    /// <summary>
    /// A <see cref="DrawableLayer"/> that inherits its source from a <see cref="Drawing.Sprite"/>.
    /// </summary>
    public class SpriteLayer : DrawableLayer
    {
        public Sprite Sprite { get; set; }

        protected override Bitmap GetBaseImage()
            => Sprite.GetImage();
    }
}
