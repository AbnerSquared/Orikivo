using System;
using System.Collections.Generic;
using System.Drawing;

namespace Orikivo.Drawing
{
    /// <summary>
    /// A custom <see cref="Bitmap"/> that supports layering and detailed configurables.
    /// </summary>
    public class Drawable : IDisposable
    {
        public Drawable(int width, int height)
        {
            Viewport = new Size(width, height);
        }

        public Drawable(Size viewport)
        {
            Viewport = viewport;
        }

        /// <summary>
        /// The initial <see cref="Point"/> that is referenced for each <see cref="DrawableLayer"/>.
        /// </summary>
        public Point Origin { get; set; } = Point.Empty;

        /// <summary>
        /// The maximum width and height for the <see cref="Bitmap"/> to render.
        /// </summary>
        public Size Viewport { get; set; }

        /// <summary>
        /// The <see cref="Bitmap"/> resolution scale that will be set when using <see cref="Build"/>.
        /// </summary>
        public ImageScale Scale { get; set; } = ImageScale.Small;

        /// <summary>
        /// The extraneous whitespace that will surround the <see cref="Drawable"/>.
        /// </summary>
        public Padding Padding { get; set; } = Padding.Empty;

        /// <summary>
        /// Represents all of the existing <see cref="DrawableLayer"/> values within the <see cref="Drawable"/>.
        /// </summary>
        public IReadOnlyList<DrawableLayer> Layers => InternalLayers;

        protected List<DrawableLayer> InternalLayers { get; } = new List<DrawableLayer>();

        /// <summary>
        /// The <see cref="GammaPalette"/> that will be applied when using <see cref="Build"/>.
        /// </summary>
        public GammaPalette Palette { get; set; } = GammaPalette.Default;

        public DrawableProperties Properties { get; set; }

        public Border Border { get; set; }

        public Size Size => new Size(Viewport.Width + Padding.Width,
            Viewport.Height + Padding.Height);

        public void SetBorder(GammaColor color, int width, BorderEdge edge = BorderEdge.Outside)
        {
            Border = new Border { Color = color, Thickness = width, Edge = edge };
        } // LeftBorder, TopBorder, RightBorder, BottomBorder

        public void ClearBorder()
        {
            Border = null;
        }

        /// <summary>
        /// Sets the origin of the <see cref="Drawable"/> to the specified <see cref="OriginAnchor"/>.
        /// </summary>
        /// <param name="anchor"></param>
        public void SetOrigin(OriginAnchor anchor)
        {
            Origin = OriginUtils.GetOrigin(Viewport, anchor);
        }

        public void SetOrigin(int x, int y)
        {
            Origin = new Point(x, y);
        }

        public void SetOrigin(Point origin)
        {
            Origin = origin;
        }

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
        public void UpdateLayer(int index, DrawableLayer layer, bool keepConfig = true)
        {
            if (keepConfig)
                layer.Config = InternalLayers[index].Config;

            InternalLayers[index] = layer;
        }

        /// <summary>
        /// Moves a <see cref="DrawableLayer"/> from its initial index to the specified index, affecting the indexes of each existing <see cref="DrawableLayer"/>.
        /// </summary>
        public void MoveLayer(int index, int newIndex)
        {
            // TODO: Figure out list shifting.
            throw new NotImplementedException();
        }

        // swap layers by index
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

        /// <summary>
        /// Compiles all of the <see cref="DrawableLayer"/> values together to return a fully rendered <see cref="Bitmap"/>.
        /// </summary>
        public Bitmap Build()
        {
            Bitmap result = new Bitmap(Size.Width, Size.Height);
            using (Graphics graphics = Graphics.FromImage(result))
            {
                foreach (DrawableLayer layer in InternalLayers)
                {
                    using (Bitmap bmp = layer.Build())
                    {
                        if (layer.Offset.X > Viewport.Width && layer.Offset.Y > Viewport.Height)
                        {
                            if (layer.Offset.X < 0 || layer.Offset.X + bmp.Width > Viewport.Width ||
                                layer.Offset.Y < 0 || layer.Offset.Y + bmp.Height > Viewport.Height)
                            {
                                Rectangle cropRect = GraphicsUtils.ClampRectangle(Origin, Viewport, layer.Offset, bmp.Size);

                                using (Bitmap crop = ImageHelper.Crop(bmp, cropRect))
                                    GraphicsUtils.ClipAndDrawImage(graphics, crop, layer.Position);

                                continue;
                            }
                        }

                        GraphicsUtils.ClipAndDrawImage(graphics, bmp, new Point(Padding.Left + layer.Offset.X, Padding.Top + layer.Offset.Y));
                    }
                }
            }

            result = ImageHelper.SetColorMap(result, GammaPalette.Default, Palette);

            if (Scale > ImageScale.Small)
                result = ImageHelper.Scale(result, (int)Scale, (int)Scale);

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
            {
                layer.Dispose();
            }
        }
    }
}
