using System;
using Orikivo.Drawing;

namespace Arcadia.Graphics
{
    /// <summary>
    /// Represents a generic card component.
    /// </summary>
    public abstract class CardComponent
    {
        public ComponentInfo Info { get; protected set; }

        public FillInfo Fill { get; protected set; }

        protected abstract DrawableLayer Build();

        /// <summary>
        /// Draws this <see cref="CardComponent"/> onto the current card instance.
        /// </summary>
        public virtual void Draw(ref Drawable card, ref Cursor cursor, ref ComponentReference previous)
        {
            if (Fill == null || Info == null)
                throw new Exception("Expected both component fill info and base info to be specified");

            DrawableLayer layer = Build();

            int offsetX = CardInfo.GetOffsetX(Info, cursor, previous);
            int offsetY = CardInfo.GetOffsetY(Info, cursor, previous);

            layer.Offset = new Coordinate(offsetX, offsetY);
            layer.Properties.Padding = Info.Padding;

            if (Info.CursorOffset.HasFlag(CursorOffset.X))
                cursor.X += layer.GetBaseWidth() + layer.Properties.Padding.Width;

            if (Info.CursorOffset.HasFlag(CursorOffset.Y))
                cursor.Y += layer.GetBaseHeight() + layer.Properties.Padding.Height;

            previous.Update(layer.GetBaseWidth(), layer.GetBaseHeight(), layer.Properties.Padding);
            card.AddLayer(layer);
        }
    }
}
