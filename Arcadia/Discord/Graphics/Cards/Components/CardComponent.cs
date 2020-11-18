using System;
using Orikivo.Drawing;

namespace Arcadia.Graphics
{
    /// <summary>
    /// Represents a generic card component.
    /// </summary>
    public abstract class CardComponent
    {
        /// <summary>
        /// Represents the base building information for this <see cref="CardComponent"/>.
        /// </summary>
        public ComponentInfo Info { get; protected set; }

        /// <summary>
        /// Represents the fill information for this <see cref="CardComponent"/>.
        /// </summary>
        public FillInfo Fill { get; protected set; }

        /// <summary>
        /// Represents the outline fill information for this <see cref="CardComponent"/>.
        /// </summary>
        public FillInfo Outline { get; internal set; }

        /// <summary>
        /// Builds the base <see cref="DrawableLayer"/> for the card.
        /// </summary>
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

            if (Info.CursorOffset.HasFlag(CursorOffset.Width))
                cursor.X += layer.GetBaseWidth();

            if (Info.CursorOffset.HasFlag(CursorOffset.PaddingWidth))
                cursor.X += layer.Properties.Padding.Width;

            if (Info.CursorOffset.HasFlag(CursorOffset.Height))
                cursor.Y += layer.GetBaseHeight();

            if (Info.CursorOffset.HasFlag(CursorOffset.PaddingHeight))
                cursor.Y += layer.Properties.Padding.Height;

            previous.Update(layer.GetBaseWidth(), layer.GetBaseHeight(), layer.Properties.Padding);
            card.AddLayer(layer);
        }
    }
}
