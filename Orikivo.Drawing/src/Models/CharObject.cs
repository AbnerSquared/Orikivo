using System.Drawing;

namespace Orikivo.Drawing
{
    /// <summary>
    /// Contains all of the required information for rendering a character within a StringCanvas.
    /// </summary>
    public struct CharObject
    {
        // can only be built inside Poxel.
        internal CharObject(Bitmap sprite, char c, System.Drawing.Point p, Size s, Padding? padding = null, Point? offset = null)
        {
            _sprite = sprite;
            Char = c;
            IsNewline = c == '\n';
            Pos = p;
            Size = s;
            Padding = padding ?? Padding.Empty;
            Offset = offset ?? Point.Empty;
        }

        public bool IsNewline { get; }
        // if specified, shows the sprite to display.
        private Bitmap _sprite;
        public Bitmap Sprite => _sprite == null ? null : _sprite.Clone(new Rectangle(0, 0, _sprite.Width, _sprite.Height), _sprite.PixelFormat);

        // the character that this is written for
        public char Char { get; }

        // the position of where this character is placed in relation to the canvas
        public Point Pos { get; }

        // the size of the character
        public Size Size { get; }

        public Padding Padding { get; }

        // the global offset used on the character.
        public Point Offset { get; }
    }
}
