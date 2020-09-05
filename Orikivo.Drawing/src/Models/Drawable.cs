using System;
using System.Collections.Generic;
using System.Drawing;

namespace Orikivo.Drawing
{
    /// <summary>
    /// Represents an image builder that supports layering.
    /// </summary>
    public class Drawable : IDisposable
    {
        private DrawableProperties _properties;

        public Drawable(int width, int height)
        {
            Viewport = new Size(width, height);
        }

        public Drawable(Unit viewport)
        {
            Viewport = viewport;
        }

        /// <summary>
        /// Gets or sets the dimensions of the visible area of this <see cref="Drawable"/>.
        /// </summary>
        public Unit Viewport { get; set; }

        /// <summary>
        /// Represents all of the existing <see cref="DrawableLayer"/> values within the <see cref="Drawable"/>.
        /// </summary>
        public IReadOnlyList<DrawableLayer> Layers => InternalLayers;

        protected List<DrawableLayer> InternalLayers { get; } = new List<DrawableLayer>();

        /// <summary>
        /// The <see cref="GammaPalette"/> that will be applied when using <see cref="Build"/>.
        /// </summary>
        public GammaPalette Palette { get; set; } = GammaPalette.Default;

        public DrawableProperties Properties
        {
            get { return _properties ??= DrawableProperties.Default; }
            set => _properties = value;
        }

        public Border Border { get; set; }

        public Unit Size => new Unit(Viewport.Width + Properties.Padding.Width,
            Viewport.Height + Properties.Padding.Height);

        /// <summary>
        /// Adds a new <see cref="DrawableLayer"/> to the set of existing layers.
        /// </summary>
        public void AddLayer(DrawableLayer layer)
        {
            // if layer is null throw new Exception();
            InternalLayers.Add(layer);
        }

        /// <summary>
        /// Updates the existing <see cref="DrawableLayer"/> at the specified index to the new one specified.
        /// </summary>
        public void UpdateLayer(int index, DrawableLayer layer, bool keepProperties = true)
        {
            if (keepProperties)
                layer.Properties = InternalLayers[index].Properties;

            InternalLayers[index] = layer;
        }

        /// <summary>
        /// Swaps two different <see cref="DrawableLayer"/> values by index.
        /// </summary>
        public void SwapLayers(int index, int newIndex)
        {
            DrawableLayer a = InternalLayers[index];
            InternalLayers[index] = InternalLayers[newIndex];
            InternalLayers[newIndex] = a;
        }

        /// <summary>
        /// Removes a <see cref="DrawableLayer"/> by its specified index.
        /// </summary>
        public void RemoveLayer(int index)
        {
            InternalLayers.RemoveAt(index);
        }

        // NOTE: This auto-resizes the Drawable to perfectly fill all layers
        /// <summary>
        /// Trims all empty pixels in this <see cref="Drawable"/> to only keep the bounding box of all layers.
        /// </summary>
        public void Trim()
        {
            int maxWidth = 0;
            int maxHeight = 0;

            foreach (DrawableLayer layer in InternalLayers)
            {
                Coordinate farthest = layer.GetFarthestPoint();

                if (farthest.X > maxWidth)
                    maxWidth = farthest.X;

                if (farthest.Y > maxHeight)
                    maxHeight = farthest.Y;
            }

            Viewport = new Unit(maxWidth, maxHeight);
        }

        /// <summary>
        /// Compiles all of the <see cref="DrawableLayer"/> values together to return a fully rendered <see cref="Bitmap"/>.
        /// </summary>
        public Bitmap Build()
        {
            var result = new Bitmap(Size.Width, Size.Height);

            using (Graphics graphics = Graphics.FromImage(result))
            {
                foreach (DrawableLayer layer in InternalLayers)
                {
                    if (layer.Disposed)
                        continue;

                    using Bitmap inner = layer.Build();

                    if (inner == null)
                        continue;

                    int x = layer.Offset.X;
                    int y = layer.Offset.Y;
                    int u = Properties.Padding.Left + x;
                    int v = Properties.Padding.Top + y;

                    if (x < 0 || x + inner.Width > Viewport.Width ||
                        y < 0 || y + inner.Height > Viewport.Height)
                    {
                        Rectangle clip = ImageHelper.ClampRectangle(Point.Empty, Viewport, layer.Offset, inner.Size);
                        using Bitmap visible = ImageHelper.Crop(inner, clip);
                        ImageHelper.ClipAndDrawImage(graphics, visible, u, v);
                        continue;
                    }

                    ImageHelper.ClipAndDrawImage(graphics, inner, u, v);
                }
            }

            if (Border != null)
                result = ImageHelper.SetBorder(result, Border.Color, Border.Thickness, Border.Edge, Border.Allow);

            if (!Properties.Margin.IsEmpty)
                result = ImageHelper.Pad(result, Properties.Margin);

            result = Properties.ColorHandling switch
            {
                DrawablePaletteHandling.Ignore => result,
                DrawablePaletteHandling.Force => ImageHelper.ForcePalette(result, Palette),
                DrawablePaletteHandling.Map => ImageHelper.SetColorMap(result, GammaPalette.Default, Palette),
                _ => result
            };

            if (Properties.Scale.X > 1 && Properties.Scale.Y > 1)
                result = ImageHelper.Scale(result, Properties.Scale.X, Properties.Scale.Y);

            return result;
        }

        public Bitmap BuildAndDispose()
        {
            Bitmap bmp = Build();
            Dispose();
            return bmp;
        }

        public virtual void Dispose()
        {
            foreach (DrawableLayer layer in InternalLayers)
                layer.Dispose();
        }
    }
}
