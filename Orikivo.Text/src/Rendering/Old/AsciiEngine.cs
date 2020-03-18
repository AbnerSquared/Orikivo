using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Text
{
    /// <summary>
    /// A rendering engine derived from ASCII characters.
    /// </summary>
    public class AsciiEngine : IDisposable
    {
		/// <summary>
        /// The default character that is drawn when an <see cref="AsciiCamera"/> is larger than the <see cref="AsciiGrid"/> it's focused on.
        /// </summary>
        public char VoidChar { get; }

		/// <summary>
        /// An optional character to set that draws the border of an <see cref="AsciiGrid"/>.
        /// </summary>
        public char? BorderChar { get; }

		/// <summary>
        /// A collection of <see cref="AsciiGrid"/> values that the <see cref="AsciiCamera"/> can focus on.
        /// </summary>
        public List<AsciiGrid> Grids { get; }

        private int _currentGridIndex = 0;

        /// <summary>
        /// The value defining the <see cref="AsciiGrid"/> that the <see cref="AsciiCamera"/> should focus on.
        /// </summary>
        public int CurrentGridIndex
        {
            get => _currentGridIndex;

            set
            {
                // cancel attempting to place a new grid out of bounds.
                if (value > (Grids.Count - 1) || value < 0)
                    return;

                _currentGridIndex = value;
            }
        }

        /// <summary>
        /// The current <see cref="AsciiGrid"/> that the <see cref="AsciiCamera"/> is focused on due to the <see cref="CurrentGridIndex"/>.
        /// </summary>
        public AsciiGrid CurrentGrid => Grids[CurrentGridIndex];

		/// <summary>
        /// The camera for the <see cref="AsciiEngine"/> that displays everything within a focused <see cref="AsciiGrid"/>.
        /// </summary>
		public AsciiCamera Camera { get; private set; }
        // TODO: Create camera trigger bound detection

        /// <summary>
        /// The refresh rate for the <see cref="AsciiCamera"/> at which the frames will be viewed at.
        /// </summary>
        public int Fps { get; set; }

        /// <summary>
        /// Creates a default <see cref="AsciiEngine"/> with a viewport width and height of 32 characters.
        /// </summary>
        public AsciiEngine() => new AsciiEngine(32, 32, ' ');

		/// <summary>
        /// Initializes a new <see cref="AsciiEngine"/> with a specified void character, viewport <paramref name="width"/>, and viewport <paramref name="height"/> (measured by character count).
        /// </summary>
		public AsciiEngine(int width, int height, char voidChar = ' ')
        {
            VoidChar = voidChar;
            Camera = new AsciiCamera(width, height);
            Grids = new List<AsciiGrid>();
            Grids.Add(new AsciiGrid(width, height));
            CurrentGridIndex = 0;
        }

        /// <summary>
        /// Initializes a new <see cref="AsciiEngine"/> with a specified void character, viewport <paramref name="width"/>, viewport <paramref name="height"/>, and a collection of <see cref="AsciiGrid"/> values at the optional current grid index.
        /// </summary>
        public AsciiEngine(int width, int height, List<AsciiGrid> grids, int currentGridIndex = 0, char voidChar = ' ')
        {
            VoidChar = voidChar;
            Camera = new AsciiCamera(width, height);

            if (grids == null)
                grids = new List<AsciiGrid>();

            if (grids.Count == 0) // if there aren't any grids.
            {
                grids.Add(new AsciiGrid(width, height));
            }

            Grids = grids;
            CurrentGridIndex = currentGridIndex;
        }

		/// <summary>
        /// Renders all of the frames with a specified range of time and incremental step.
        /// </summary>
		public string[] GetFrames(int fromTime, int toTime, float step)
        {
            // TODO: Make from/toTime floats, and make sure that they are divided evenly.
            if (toTime - fromTime < 1)
                throw new ArgumentException("The time range specified must at least have a difference of 1.");

            //string[] frames = new string[Utils.GetFrameCount(fromTime, toTime, step)];
            List<string> frames = new List<string>();
            int frameIndex = 0;
            for (float i = fromTime; i <= toTime; i += step) // from its intital time to its end time, increment by step
            {
                Console.WriteLine($"Frame.Index: {frameIndex}");
                Console.WriteLine($"Time.Index: {i}");
                frames.Add(GetFrame(i));
                //frames[frameIndex] = GetFrame(i); // get the frame at the specified time.
                frameIndex++;
            }

            return frames.ToArray();
        }

        /// <summary>
        /// Renders all of the frames for a specified duration and incremental step (with its default being 1).
        /// </summary>
        public string[] GetFrames(int time, float step = 1) // TODO: Calculate FPS
            => GetFrames(0, time, step);

        /// <summary>
        /// Gets the frame at the specified time.
        /// </summary>
        public string GetFrame(float time)
        {
            return CurrentGrid.ToString(time);
        }

		public void AddObject(AsciiObject asciiObj)
        {
            CurrentGrid.Objects.Add(asciiObj);
            // TODO: Handle collision when attempting to stack objects on top of each other.
        }

        public void Dispose()
        {
            // TODO: Add disposal methods.
        }
    }
}
