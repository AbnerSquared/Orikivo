using System;
using System.Drawing;

namespace Orikivo.Drawing
{
    public class CachedChar : IDisposable
    {
        public CachedChar(int fontIndex, Bitmap value)
        {
            FontIndex = fontIndex;
            Value = value;
            Disposed = false;
        }

        // NOTE: The font index of this character, relative to the TextFactory's FontIndex.
        public int FontIndex { get; internal set; }

        // NOTE: The image value for the char
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
