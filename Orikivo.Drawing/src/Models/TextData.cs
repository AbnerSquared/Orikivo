using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Drawing
{
    internal class TextData : IDisposable
    {
        internal TextData(string text, Padding padding, int width, int height, IEnumerable<CharData> chars)
        {
            Content = text;
            Padding = padding;
            Width = width;
            Height = height;
            Chars = chars;
        }

        public string Content { get; }

        public bool Disposed { get; private set; } = false;

        public Padding Padding { get; }

        public int ImageWidth => Padding.Width + Width;

        public int ImageHeight => Padding.Height + Height;

        public int Width { get; }

        public int Height { get; }

        public IEnumerable<CharData> Chars { get; }

        public void Dispose()
        {
            if (Disposed)
                return;

            if (Chars == null || Chars?.Count() == 0)
            {
                Disposed = true;
                return;
            }

            foreach (CharData data in Chars)
                data.Dispose();

            Disposed = true;
        }
    }
}
