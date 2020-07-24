using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Orikivo.Drawing.Encoding;

namespace Orikivo.Drawing.Animating
{
    public class Animator : IAnimator
    {
        public Animator() { }

        public Animator(TimeSpan frameLength, int width, int height, int? repeatCount = null)
        {
            RepeatCount = repeatCount;
            FrameLength = frameLength;
            Viewport = new Size(width, height);
        }

        public Animator(TimeSpan frameLength, Size viewport, int? repeatCount = null)
        {
            RepeatCount = repeatCount;
            FrameLength = frameLength;
            Viewport = viewport;
        
        }

        public int? RepeatCount { get; set; }

        public TimeSpan FrameLength { get; set; }

        public Size Viewport { get; set; }

        public List<Frame> Frames { get; } = new List<Frame>();

        public bool Disposed { get; protected set; }

        public void AddFrame(Frame frame)
            => Frames.Add(frame);

        public void AddFrames(IEnumerable<Frame> frames)
            => Frames.AddRange(frames);

        public void AddFrames(params Frame[] frames)
            => Frames.AddRange(frames);

        public void UpdateFrameAt(int index, Frame frame)
            => Frames[index] = frame;

        public void RemoveFrameAt(int index)
            => Frames.RemoveAt(index);

        public MemoryStream Compile(TimeSpan frameLength, Quality quality = Quality.Bpp8)
        {
            var animation = new MemoryStream();
            using (var encoder = new GifEncoder(animation, Viewport, RepeatCount))
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

            foreach (Frame frame in Frames)
                frame.Dispose();

            Disposed = true;
        }
    }
}
