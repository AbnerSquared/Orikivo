using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Discord;

namespace Orikivo
{
    // this sets up how the console window is built; such as bg and text color, font used, window size, position, and title.
    public class OriConsoleConfig
    {
        public static OriConsoleConfig Default
        {
            get
            {
                OriConsoleConfig defaultConsoleConfig = new OriConsoleConfig();
                defaultConsoleConfig.BackgroundColor = ConsoleColor.DarkCyan;
                defaultConsoleConfig.TextColor = ConsoleColor.Cyan;
                defaultConsoleConfig.ShowCursor = false;
                // screen size is 1920 x 1080
                defaultConsoleConfig.WindowSize = null;
                defaultConsoleConfig.WindowPosition = null;
                return defaultConsoleConfig;
            }
        }

        public string Title { get; set; }
        public ConsoleColor? BackgroundColor { get; set; } // bg color should be optional
        public ConsoleColor? TextColor { get; set; }
        public bool ShowCursor { get; set; }
        public bool Debug { get; set; } // if the bot writes any debug console event
        public string OutputPath { get; set; } // where the console saves all events, if empty, it will not log
        public Point? WindowPosition { get; set; }
        public Size? WindowSize { get; set; }
    }
}
