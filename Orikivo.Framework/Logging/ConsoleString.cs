using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo.Framework
{
    /// <summary>
    /// Represents a <see cref="string"/> with assignable colors.
    /// </summary>
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
