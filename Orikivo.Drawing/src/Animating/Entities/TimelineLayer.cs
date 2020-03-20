using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Orikivo.Drawing.Animating
{
    public class TimelineLayer : IDisposable
    {
        public TimelineLayer(Bitmap source, IEnumerable<Keyframe> keyframes, long startTick, long endTick)
        {
            Source = source;
            Keyframes = keyframes.ToList();
            StartTick = startTick;
            EndTick = endTick;
        }

        public Bitmap Source { get; }

        public IReadOnlyList<Keyframe> Keyframes { get; }

        public long StartTick { get; }

        public long EndTick { get; }

        public long Length => EndTick - StartTick;

        public bool Disposed { get; protected set; } = false;

        public Keyframe KeyframeAt(long tick)
            => Keyframes?.Count > 0
            ? GetCurrentKeyframe(GetLastKeyframe(tick), GetNextKeyframe(tick), tick)
            : Keyframe.GetDefault(StartTick);

        private Keyframe GetLastKeyframe(long tick)
            => tick > StartTick && Keyframes?
                .Where(x => x.Tick < tick)
                .Count() > 0
                ? Keyframes?
                    .Where(x => x.Tick < tick)
                    .OrderBy(x => Math.Abs(x.Tick - tick))
                    .FirstOrDefault()
                    ?? Keyframe.GetDefault(StartTick)
                : Keyframe.GetDefault(StartTick);

        private Keyframe GetNextKeyframe(long tick)
            => Keyframes?
                .Where(x => x.Tick >= tick)
                .OrderBy(x => Math.Abs(x.Tick - tick))
                .FirstOrDefault()
                ?? Keyframe.GetDefault(tick);

        private Keyframe GetCurrentKeyframe(Keyframe last, Keyframe next, long tick)
        {
            float progress = GetTickProgress(last.Tick, next.Tick, tick);
            float opacity = GetCurrentValue(last.Opacity, next.Opacity, progress);
            float rotation = GetCurrentValue(last.Rotation, next.Rotation, progress);
            
            Vector2 position = new Vector2(
                GetCurrentValue(last.Position.X, next.Position.X, progress),
                GetCurrentValue(last.Position.Y, next.Position.Y, progress));
            
            Vector2 scale = new Vector2(
                GetCurrentValue(last.Scale.X, next.Scale.X, progress),
                GetCurrentValue(last.Scale.Y, next.Scale.Y, progress));

            return new Keyframe(tick, opacity, position, rotation, scale);
        }

        private float GetTickProgress(long last, long next, long current)
            => RangeF.Convert(last, next, 0.0f, 1.0f, current);

        private float GetCurrentValue(float last, float next, float progress)
            => RangeF.Convert(0.0f, 1.0f, last, next, progress);

        public void Dispose()
        {
            if (Disposed)
                return;

            Source.Dispose();
            Disposed = true;
        }
    }
}
