using Orikivo.Drawing;

namespace Arcadia.Graphics
{
    public interface ICardComponent
    {
        ComponentInfo Info { get; }
        FillInfo Fill { get; }
        void Draw(Drawable card, ref Cursor cursor, ref ComponentReference previous);
    }
}