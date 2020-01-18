using System;
using System.Drawing;

namespace Orikivo
{
    /// <summary>
    /// A configuration class that controls how the <see cref="Console"/> is drawn.
    /// </summary>
    public class ConsoleConfig
    {
        /// <summary>
        /// Gets the default <see cref="ConsoleConfig"/>.
        /// </summary>
        public static ConsoleConfig Default
        {
            get
            {
                return new ConsoleConfig
                {
                    BackgroundColor = ConsoleColor.DarkCyan,
                    TextColor = ConsoleColor.Cyan,
                    ShowCursor = false,
                    WindowSize = null,
                    WindowPosition = null,
                    Debug = true,
                    OutputPath = null
                };
            }
        }

        /// <summary>
        /// The title of the <see cref="Console"/> window to use.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// An optional <see cref="ConsoleColor"/> value for the <see cref="Console.BackgroundColor"/>.
        /// </summary>
        public ConsoleColor? BackgroundColor { get; set; }

        /// <summary>
        /// An optional <see cref="ConsoleColor"/> value for the <see cref="Console.TextColor"/>.
        /// </summary>
        public ConsoleColor? TextColor { get; set; }

        /// <summary>
        /// A <see cref="bool"/> value that states if the <see cref="Console"/> should display the cursor.
        /// </summary>
        public bool ShowCursor { get; set; }

        /// <summary>
        /// A <see cref="Point"/> that marks the top-left position of the <see cref="Console"/> window.
        /// </summary>
        public Point? WindowPosition { get; set; }

        /// <summary>
        /// A <see cref="Size"/> that states the width and height of the <see cref="Console"/>.
        /// </summary>
        public Size? WindowSize { get; set; }

        /// <summary>
        /// A <see cref="bool"/> value that states if the <see cref="Console"/> should be logging debug events.
        /// </summary>
        public bool Debug { get; set; }

        /// <summary>
        /// A local path that states the directory of where logs should be stored. If left empty, the <see cref="Console"/> will not store any logs.
        /// </summary>
        public string OutputPath { get; set; }
    }
}
