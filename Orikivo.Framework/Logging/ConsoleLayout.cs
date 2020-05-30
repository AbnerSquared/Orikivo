using System;
using System.Drawing;

namespace Orikivo.Framework
{
    /// <summary>
    /// Represents the visual configuration for the <see cref="Console"/>.
    /// </summary>
    public class ConsoleLayout
    {
        public static ConsoleLayout GetCurrent()
        {
            return new ConsoleLayout
            {
                Position = new Point(Console.WindowLeft, Console.WindowTop),
                Size = new Size(Console.WindowWidth, Console.WindowWidth),
                Title = Console.Title,
                BackgroundColor = Console.BackgroundColor,
                ForegroundColor = Console.ForegroundColor,
                CursorVisible = Console.CursorVisible,
                Cursor = new Point(Console.CursorLeft, Console.CursorTop),
                CursorSize = Console.CursorSize
            };
        }

        public string Title { get; set; }

        public ConsoleColor? BackgroundColor { get; set; }

        public ConsoleColor? ForegroundColor { get; set; }

        public bool CursorVisible { get; set; }

        public Point? Position { get; set; }

        public Point? Cursor { get; set; }

        public int? CursorSize { get; set; }

        public Size? Size { get; set; }

        public void Set()
        {
            if (Position.HasValue)
                Console.SetWindowPosition(Position.Value.X, Position.Value.Y);

            if (Size.HasValue)
                Console.SetWindowSize(Size.Value.Width, Size.Value.Height);
            
            if (!string.IsNullOrWhiteSpace(Title))
                Console.Title = Title;

            if (BackgroundColor.HasValue)
                Console.BackgroundColor = BackgroundColor.Value;

            if (ForegroundColor.HasValue)
                Console.ForegroundColor = ForegroundColor.Value;

            Console.CursorVisible = CursorVisible;

            if (Cursor.HasValue)
                Console.SetCursorPosition(Cursor.Value.X, Cursor.Value.Y);

            if (CursorSize.HasValue)
                Console.CursorSize = CursorSize.Value;
        }
    }
}
