﻿using System;
using System.Drawing;

namespace Orikivo.Drawing
{
    // Revert the implementation of Properties; Instead, use the properties class to set initial values, and then edit from there.
    public abstract class DrawableLayer : IDisposable
    {
        private DrawableProperties _properties;

        /// <summary>
        /// Gets or sets the max width that this <see cref="DrawableLayer"/> can be set to.
        /// </summary>
        public int? MaxWidth { get; set; }

        /// <summary>
        /// Gets or sets the max height the <see cref="DrawableLayer"/> can be set to.
        /// </summary>
        public int? MaxHeight { get; set; }

        /// <summary>
        /// Gets or sets the initial offset for this <see cref="DrawableLayer"/>.
        /// </summary>
        public Coordinate Offset { get; set; } = Coordinate.Empty;

        public DrawableProperties Properties
        {
            get => _properties ?? DrawableProperties.Default;
            set => _properties = value;
        }

        internal bool Disposed { get; set; }

        internal Coordinate GetFarthestPoint()
        {
            var position = new Coordinate(Offset.X + Properties.Padding.Left, Offset.Y + Properties.Padding.Top);
            using Bitmap image = GetBaseImage();

            if (image == null)
                return position;

            int width = image.Width;
            int height = image.Height;

            if (MaxWidth.HasValue)
                if (width > MaxWidth.Value)
                    width = MaxWidth.Value;

            if (MaxHeight.HasValue)
                if (height > MaxHeight.Value)
                    height = MaxHeight.Value;

            position.X += width;
            position.Y += height;

            return position;
        }

        public int GetBaseWidth()
        {
            using Bitmap image = GetBaseImage();

            if (image == null)
                return 0;

            return image.Width;
        }

        public int GetBaseHeight()
        {
            using Bitmap image = GetBaseImage();

            if (image == null)
                return 0;

            return image.Height;
        }

        /// <summary>
        /// Renders the base image for this <see cref="DrawableLayer"/>.
        /// </summary>
        protected abstract Bitmap GetBaseImage();

        /// <summary>
        /// Renders the finalized <see cref="Bitmap"/> for this <see cref="DrawableLayer"/>.
        /// </summary>
        public virtual Bitmap Build()
        {
            if (Disposed)
                throw new ObjectDisposedException("The layer has already been disposed.");

            using (Bitmap image = GetBaseImage())
            {
                if (image == null)
                    return null;

                int width = image.Width;
                int height = image.Height;

                if (MaxWidth.HasValue)
                    if (width > MaxWidth.Value)
                        width = MaxWidth.Value;

                if (MaxHeight.HasValue)
                    if (height > MaxHeight.Value)
                        height = MaxHeight.Value;

                var result = new Bitmap(width + Properties.Padding.Width, height + Properties.Padding.Height);

                using (Graphics graphics = Graphics.FromImage(result))
                {
                    ImageHelper.ClipAndDrawImage(graphics, image, Properties.Padding.Left, Properties.Padding.Top);

                    // TODO: Implement ImageConfig manipulation.
                }

                return result;
            }
        }

        public virtual void Dispose() { }
    }
}
