using System;
using System.Drawing;

namespace Orikivo.Drawing
{
    /// <summary>
    /// Contains all of the required information for rendering a character.
    /// </summary>
    internal struct CharData : IDisposable
    {
        private readonly Bitmap _sprite;

        internal CharData(Bitmap sprite, char c, Point p, Unit u, Padding? padding = null, Point? offset = null)
        {
            Disposed = false;
            _sprite = sprite;
            Char = c;
            IsNewline = c == '\n';
            Position = p;
            Size = u;
            Padding = padding ?? Padding.Empty;
            Offset = offset ?? Point.Empty;
        }

        internal CharData(Bitmap sprite, char c, int x, int y, int width, int height, Padding? padding = null, Point? offset = null)
        {
            Disposed = false;
            _sprite = sprite;
            Char = c;
            IsNewline = c == '\n';
            Position = new Point(x, y);
            Size = new Unit(width, height);
            Padding = padding ?? Padding.Empty;
            Offset = offset ?? Point.Empty;
        }

        public bool IsNewline { get; }

        /// <summary>
        /// Gets the character that was used to initialize this <see cref="CharData"/>.
        /// </summary>
        public char Char { get; }

        /// <summary>
        /// Gets the position of where this <see cref="CharData"/> will be placed in relation to an <see cref="Image"/>.
        /// </summary>
        public Point Position { get; }

        /// <summary>
        /// Gets the width and height of this <see cref="CharData"/>.
        /// </summary>
        public Unit Size { get; }

        /// <summary>
        /// Gets the padding of this <see cref="CharData"/>.
        /// </summary>
        public Padding Padding { get; }

        /// <summary>
        /// Gets the offset of this <see cref="CharData"/> in relation to an <see cref="Image"/>.
        /// </summary>
        public Point Offset { get; }

        public bool Disposed { get; private set; }

        public bool HasSprite()
            => _sprite != null || Disposed;

        /// <summary>
        /// Attempts to get the <see cref="Bitmap"/> stored from this <see cref="CharData"/>.
        /// </summary>
        public Bitmap GetSprite()
            => _sprite?.Clone(new Rectangle(0, 0, _sprite.Width, _sprite.Height), _sprite.PixelFormat);

        public void Dispose()
        {
            if (Disposed)
                return;

            if (_sprite != null)
            {
                _sprite.Dispose();
                Disposed = true;
            }
        }
    }
}
