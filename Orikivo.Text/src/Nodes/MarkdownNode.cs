namespace Orikivo.Text.Nodes
{
    public class MarkdownNode
    {
        public string Value;
        public Markdown Flag { get; set; }

        public bool Bold; // **VALUE**
        public bool Italics; // *VALUE*
        public bool Strike; // ~~VALUE~~
        public bool Underline; // __VALUE__
        public bool Spoiler; // ||VALUE||
        public bool Code; // `VALUE` OR ```VALUE```
        public bool Quote; // > VALUE
    }
}
