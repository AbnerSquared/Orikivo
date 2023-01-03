namespace Arcadia
{
    /*
     NOTE: Formatting for a header is written as:
     > Icon **Title: Group** Extra
     > Subtitle 
    */
    public class Header
    {
        public Header() { }

        public Header(Header header)
        {
            Title = header.Title;
            Group = header.Group;
            Extra = header.Extra;
            Icon = header.Icon;
            Subtitle = header.Subtitle;
        }

        public string Title { get; set; }

        public string Group { get; set; }

        public string Extra { get; set; }

        public string Icon { get; set; }

        public string Subtitle { get; set; }
    }
}
