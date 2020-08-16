using System;
using System.Drawing;

namespace Orikivo.Drawing
{
    internal class CachedChar : IDisposable
    {
        public CachedChar(int fontIndex, Bitmap value)
        {
            FontIndex = fontIndex;
            Value = value ?? throw new ArgumentNullException(nameof(value), "Cannot cache a sprite for a character if the specified image is null");
            Disposed = false;
        }

        public int FontIndex { get; internal set; }

        public Bitmap Value { get; internal set; }

        public bool Disposed { get; private set; }

        public void Dispose()
        {
            if (Disposed)
                return;

            Value.Dispose();

            Disposed = true;
        }
    }
}
