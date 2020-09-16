using System;
using System.Text;

namespace Orikivo.Text.Nodes
{
    [Flags]
    public enum Markdown
    {
        Bold = 1,
        Italics = 2,
        Strike = 4,
        Underline = 8,
        Code = 16,
        Quote = 32,
        Spoiler = 64
    }

    public class MarkdownBuilder
    {
        private readonly StringBuilder _builder;

        public MarkdownBuilder()
        {
            _builder = new StringBuilder();
        }

        internal static string Format(string input, Markdown markdown)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            var format = new StringBuilder("{0}");

            if (markdown == 0)
                return format.ToString();

            if (markdown.HasFlag(Markdown.Code))
            {
                if (markdown.HasFlag(Markdown.Quote))
                {
                    format.Prepend("> ");
                }

                if (input.Contains('\n'))
                {
                    format.Encase("```");
                }
                else
                    format.Encase('`');
            }
            else if (markdown.HasFlag(Markdown.Quote))
            {
                if (input.Contains('\n'))
                {
                    format.Prepend(">>> ");
                }
            }

            if (markdown.HasFlag(Markdown.Bold))
            {
                format.Encase("**");
            }

            if (markdown.HasFlag(Markdown.Italics))
                format.Encase('*');

            if (markdown.HasFlag(Markdown.Strike))
                format.Encase("~~");

            if (markdown.HasFlag(Markdown.Underline))
            {
                format.Encase("__");
            }

            if (markdown.HasFlag(Markdown.Spoiler))
            {
                format.Encase("||");
            }

            return format.ToString();
        }
    }
}
