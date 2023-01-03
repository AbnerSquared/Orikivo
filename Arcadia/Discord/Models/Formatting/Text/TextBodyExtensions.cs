namespace Arcadia
{
    public static class TextBodyExtensions
    {
        public static TextBody WithSection(this TextBody body, string title, string content, string icon = null)
        {
            body.Sections.Add(new TextSection { Title = title, Content = content, Icon = icon });
            return body;
        }

        public static TextBody WithSection(this TextBody body, TextSection section)
        {
            body.Sections.Add(section);
            return body;
        }

        public static TextBody WithHeader(this TextBody body, string title, string group = null, string extra = null, string icon = null, string subtitle = null)
        {
            body.Header = Locale.CreateHeader(title, group, extra, icon, subtitle);
            return body;
        }

        public static TextBody WithHeader(this TextBody body, Header header)
        {
            body.Header = header;
            return body;
        }

        public static TextBody AppendTip(this TextBody body, string tip)
        {
            body.Tooltips.Add(tip);
            return body;
        }
    }
}
