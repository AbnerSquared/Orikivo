using System.Text;
using Orikivo.Text.Nodes;

namespace Orikivo.Text
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder Prepend(this StringBuilder builder, string value)
        {
            return builder.Insert(0, value);
        }

        public static StringBuilder Prepend(this StringBuilder builder, char value)
        {
            return builder.Insert(0, value);
        }

        public static StringBuilder PrependLine(this StringBuilder builder)
        {
            return builder.Insert(0, "\n");
        }

        public static StringBuilder PrependLine(this StringBuilder builder, string value)
        {
            return builder.Insert(0, $"{value}\n");
        }

        // Wraps the current value in the string builder by a specified value
        public static StringBuilder Encase(this StringBuilder builder, string s)
        {
            return builder.Insert(0, s).Append(s);
        }

        public static StringBuilder Encase(this StringBuilder builder, char c)
        {
            return builder.Insert(0, c).Append(c);
        }

        /// <summary>
        /// Appends a specified number of copies of the specified string to this instance.
        /// </summary>
        public static StringBuilder Append(this StringBuilder stringBuilder, string value, int repeatCount)
        {
            for (int i = 0; i < repeatCount; i++)
                stringBuilder.Append(value);

            return stringBuilder;
        }

        public static StringBuilder AppendMarkdown(this StringBuilder builder, string value, Markdown markdown = 0)
        {
            return builder.Append(MarkdownBuilder.Format(value, markdown));
        }
    }
}