using System;
using System.Collections.Generic;
using System.Drawing;

namespace Orikivo.Drawing.Graphics2D
{
    /// <summary>
    /// A simulator that follows the rules of Conway's Game of Life.
    /// </summary>
    public class ConwayRenderer // TODO: Create the ability to wrap around.
    {
        public ConwayRenderer(GammaColor liveColor, GammaColor deadColor, ulong? decayTickLength, Grid<ConwayCell> pattern)
        {
            LiveColor = liveColor;
            DeadColor = deadColor;
            DecayTickLength = decayTickLength;
            Pattern = CurrentGeneration = pattern;
        }

        private GammaColor? _activeColor;

        /// <summary>
        /// The <see cref="GammaColor"/> that represents a currently living <see cref="ConwayCell"/>. If left empty, it will default to <see cref="LiveColor"/>.
        /// </summary>
        public GammaColor ActiveColor { get => _activeColor.GetValueOrDefault(LiveColor); set => _activeColor = value; }

        /// <summary>
        /// The <see cref="GammaColor"/> that represents a living <see cref="ConwayCell"/>.
        /// </summary>
        public GammaColor LiveColor { get; }

        /// <summary>
        /// The <see cref="GammaColor"/> that represents a dead <see cref="ConwayCell"/>.
        /// </summary>
        public GammaColor DeadColor { get; }

        /// <summary>
        /// The amount of ticks required before the <see cref="LiveColor"/> is set to the <see cref="DeadColor"/>.
        /// </summary>
        public ulong? DecayTickLength { get; } = 0;

        /// <summary>
        /// The initial pattern used to determine the generation cycles for each <see cref="ConwayCell"/>.
        /// </summary>
        public Grid<ConwayCell> Pattern { get; }

        public Grid<ConwayCell> CurrentGeneration { get; private set; }

        public long CurrentTick { get; private set; } = 0;

        public int Height => Pattern.Height;
        public int Width => Pattern.Width;

        public static Grid<ConwayCell> GetRandomPattern(int width, int height)
        {
            Grid<ConwayCell> pattern = new Grid<ConwayCell>(width, height);

            pattern.SetEachValue(delegate
            {
                return ConwayCell.FromRandom();
            });

            return pattern;
        }

        private int GetNeighborCount(int x, int y) // TODO: Could be condensed somehow.
        {
            RangeF width = new RangeF(0, Width, true, false);
            RangeF height = new RangeF(0, Height, true, false);

            if (!width.Contains(x) || !height.Contains(y))
                throw new Exception("The specified point to check are out of bounds.");

            int up = y - 1;
            int down = y + 1;
            int left = x - 1;
            int right = x + 1;

            bool canPeekLeft = width.Contains(left);
            bool canPeekUp = height.Contains(up);
            bool canPeekDown = height.Contains(down);
            bool canPeekRight = width.Contains(right);

            int neighbors = 0;

            if (canPeekUp)
            {
                if (canPeekLeft)
                    if (CurrentGeneration[left, up].Active)
                        neighbors++;

                if (CurrentGeneration[x, up].Active)
                    neighbors++;

                if (canPeekRight)
                    if (CurrentGeneration[right, up].Active)
                        neighbors++;
            }

            if (canPeekLeft)
                if (CurrentGeneration[left, y].Active)
                    neighbors++;

            if (canPeekRight)
                if (CurrentGeneration[right, y].Active)
                    neighbors++;

            if (canPeekDown)
            {
                if (canPeekLeft)
                    if (CurrentGeneration[left, down].Active)
                        neighbors++;

                if (CurrentGeneration[x, down].Active)
                    neighbors++;

                if (canPeekRight)
                    if (CurrentGeneration[right, down].Active)
                        neighbors++;
            }

            return neighbors;
        }

        public ConwayCell GetNextCell(int x, int y)
        {
            ConwayCell last = CurrentGeneration[x, y];
            ConwayCell next = last.Clone();

            int lastNeighbors = GetNeighborCount(x, y);

            if (lastNeighbors == 2 || lastNeighbors == 3)
            {
                if (lastNeighbors == 3 && !last.Active)
                    next.Toggle();

                next.LastActiveTick = CurrentTick + 1;
            }
            else if (last.Active)
                next.Toggle();

            return next;
        }

        public Grid<ConwayCell> GetNextGeneration()
        {
            Grid<ConwayCell> next = new Grid<ConwayCell>(Width, Height);

            next.SetEachValue((int x, int y) => GetNextCell(x, y));

            return next;
        }

        public List<Grid<Color>> Run(long ticks)
        {
            List<Grid<Color>> frames = new List<Grid<Color>>();

            if (CurrentTick == 0)
            {
                frames.Add(GetPixels());
            }

            for (int t = CurrentTick == 0 ? 1 : 0; t < ticks; t++)
            {
                CurrentGeneration = GetNextGeneration();
                CurrentTick++;
                frames.Add(GetPixels());
            }

            return frames;
        }

        public Grid<Color> GetPixels()
        {
            Grid<Color> grid = new Grid<Color>(Width, Height);

            grid.SetEachValue(delegate (int x, int y)
            {
                ConwayCell cell = CurrentGeneration[x, y];

                if (!cell.Initialized)
                {
                    return DeadColor;
                }
                else if (cell.LastActiveTick == CurrentTick)
                {
                    return ActiveColor;
                }
                else if (DecayTickLength.GetValueOrDefault(0) > 0)
                {
                    long lastActive = CurrentTick - cell.LastActiveTick;

                    float remainder = 1.0f - RangeF.Convert(0, DecayTickLength.Value + 1, 0.0f, 1.0f,
                                   RangeF.Clamp(0, DecayTickLength.Value + 1, lastActive));

                    return GammaColor.Merge(DeadColor, LiveColor, remainder);
                }
                else
                {
                    return cell.LastActiveTick == CurrentTick ? LiveColor : DeadColor;
                }
            });

            return grid;
        }
    }
}
