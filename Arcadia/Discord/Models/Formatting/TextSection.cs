using System.Text;
using Orikivo;

namespace Arcadia
{
    public class TextSection
    {
        public string Icon { get; set; }

        public string Title { get; set; }

        public virtual string Content { get; set; }

        public string Build()
        {
            var result = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(Title))
                result.AppendLine(Format.Header(Title, Icon, useMarkdown: false));

            if (!string.IsNullOrWhiteSpace(Content))
                result.AppendLine(Content);

            return result.ToString();
        }
    }
}