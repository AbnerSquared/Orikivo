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
        /// Returns the default <see cref="ConsoleConfig"/>.
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
                    Size = null,
                    Position = null,
                    Debug = true,
                    OutputPath = null
                };
            }
        }

        /// <summary>
        /// Gets or sets the title of the <see cref="Console"/> window to use.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets an optional <see cref="ConsoleColor"/> value for the <see cref="Console.BackgroundColor"/>.
        /// </summary>
        public ConsoleColor? BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets an optional <see cref="ConsoleColor"/> value for the <see cref="Console.ForegroundColor"/>.
        /// </summary>
        public ConsoleColor? TextColor { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="bool"/> value that determines if the <see cref="Console"/> should display the cursor.
        /// </summary>
        public bool ShowCursor { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="Point"/> that represents the top-left position of the <see cref="Console"/> window.
        /// </summary>
        public Point? Position { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="System.Drawing.Size"/> that states the width and height of the <see cref="Console"/>.
        /// </summary>
        public Size? Size { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="bool"/> value that determines if the <see cref="Console"/> should be logging debug events.
        /// </summary>
        public bool Debug { get; set; }

        /// <summary>
        /// Gets or sets a local path that states the directory of where logs should be stored. If left empty, the <see cref="Console"/> will not store any logs.
        /// </summary>
        public string OutputPath { get; set; }
    }
}
