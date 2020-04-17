using System;

namespace Orikivo.Framework
{
    public struct ConsoleString
    {
        public ConsoleString(string content, ConsoleColor? color = null, ConsoleColor? highlight = null)
        {
            Content = content;
            Color = color;
            Highlight = highlight;
        }

        public string Content { get; set; }

        public ConsoleColor? Color { get; set; }

        public ConsoleColor? Highlight { get; set; }

        public static implicit operator ConsoleString(string content)
            => new ConsoleString(content, null);
    }
}
