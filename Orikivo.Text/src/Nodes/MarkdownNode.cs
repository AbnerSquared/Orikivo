namespace Orikivo.Text.Nodes
{
    public class MarkdownNode
    {
        public string Value;
        public Markdown Flag { get; set; }

        public bool Bold;
        public bool Italics;
        public bool Strike;
        public bool Underline;
        public bool Spoiler;
        public bool Code;
        public bool Quote;
    }
}
