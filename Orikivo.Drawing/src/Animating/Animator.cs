using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Orikivo.Drawing.Encoding;

namespace Orikivo.Drawing.Animating
{
    public class Animator : IAnimator
    {
        private List<Frame> _frames = new List<Frame>();

        public int? RepeatCount { get; set; }

        public TimeSpan FrameLength { get; set; }

        public Size Viewport { get; set; }

        public IReadOnlyList<Frame> Frames => _frames;

        public bool Disposed { get; protected set; } = false;

        public void AddFrame(Frame frame)
            => _frames.Add(frame);

        public void AddFrames(IEnumerable<Frame> frames)
            => _frames.AddRange(frames);

        public void AddFrames(params Frame[] frames)
            => _frames.AddRange(frames);

        public void UpdateFrameAt(int index, Frame frame)
            => _frames[index] = frame;

        public void RemoveFrameAt(int index)
            => _frames.RemoveAt(index);

        public MemoryStream Compile(TimeSpan frameLength, Quality quality = Quality.Bpp8)
        {
            MemoryStream animation = new MemoryStream();
            using (GifEncoder encoder = new GifEncoder(animation, Viewport, RepeatCount))
            {
                encoder.FrameLength = frameLength;
                encoder.Quality = quality;

                foreach (Frame frame in Frames)
                    encoder.EncodeFrame(frame.Source, frameLength: frame.Length);
            }

            animation.Position = 0;
            return animation;
        }

        public void Dispose()
        {
            if (Disposed)
                return;

            foreach (Frame frame in _frames)
                frame.Dispose();

            Disposed = true;
        }
    }
}
