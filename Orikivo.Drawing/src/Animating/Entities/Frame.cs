using System;
using System.Drawing;

namespace Orikivo.Drawing.Animating
{
    public class Frame : IDisposable
    {
        public Frame(Bitmap source, TimeSpan? length = null)
        {
            Source = source;
            Length = length;
        }

        public Bitmap Source { get; }

        public TimeSpan? Length { get; set; }

        public ImageProperties Config { get; set; }

        public bool Disposed { get; protected set; } = false;

        public void Dispose()
        {
            if (Disposed)
                return;

            Source.Dispose();
            Disposed = true;
        }
    }
}
