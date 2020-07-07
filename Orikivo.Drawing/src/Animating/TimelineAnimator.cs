using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Orikivo.Drawing.Encoding;

namespace Orikivo.Drawing.Animating
{
    public class TimelineAnimator : IAnimator
    {
        private List<TimelineLayer> _layers = new List<TimelineLayer>();

        public int? RepeatCount { get; set; }

        public TimeSpan FrameLength { get; set; }

        public Size Viewport { get; set; }

        public long Ticks { get; set; }

        public IReadOnlyList<TimelineLayer> Layers => _layers;

        public bool Disposed { get; protected set; } = false;

        public void AddLayer(TimelineLayer layer)
            => _layers.Add(layer);

        public MemoryStream Compile(TimeSpan frameLength, Quality quality = Quality.Bpp8)
        {
            if (Disposed)
                throw new ObjectDisposedException("Layers");

            MemoryStream animation = new MemoryStream();

            using (GifEncoder encoder = new GifEncoder(animation, Viewport.Width, Viewport.Height))
            {
                encoder.FrameLength = frameLength;
                encoder.Quality = quality;

                foreach (Frame frame in CompileFrames())
                {
                    using (frame)
                        encoder.EncodeFrame(frame.Source, frameLength: frame.Length);
                }
            }

            animation.Position = 0;
            return animation;
        }

        public Frame CompileFrameAt(long tick)
        {
            Bitmap frame = new Bitmap(Viewport.Width, Viewport.Height);
            
            using Graphics g = Graphics.FromImage(frame);

            for (int i = 0; i < Layers.Count; i++)
            {
                if (Layers[i].EndTick < tick || Layers[i].StartTick > tick)
                    continue;

                Keyframe keyframe = Layers[i].KeyframeAt(tick);

                using Bitmap layer = ImageHelper.Transform(
                    Viewport,
                    Layers[i].Source,
                    keyframe.Transform,
                    keyframe.Opacity);

                GraphicsUtils.ClipAndDrawImage(g, layer, Point.Empty);
            }

            return new Frame(frame);
        }

        private IEnumerable<Frame> CompileFrames()
        {
            for (long t = 0; t < Ticks; t++)
                yield return CompileFrameAt(t);
        }

        public void Dispose()
        {
            if (Disposed)
                return;

            foreach (TimelineLayer layer in Layers)
                layer.Dispose();

            Disposed = true;
        }
    }
}
