using System;
using System.Drawing;
using System.IO;
using Orikivo.Drawing.Encoding;

namespace Orikivo.Drawing.Animating
{
    public interface IAnimator : IDisposable
    {
        int? RepeatCount { get; set; }

        TimeSpan FrameLength { get; set; }

        Size Viewport { get; set; }

        bool Disposed { get; }

        MemoryStream Compile(TimeSpan frameLength, Quality quality = Quality.Bpp8);
    }
}
