using Orikivo.Text;
using System.Collections.Generic;

namespace Arcadia
{
    public class TextBody
    {
        public Language Language { get; set; } = Language.English;

        public List<string> Tooltips { get; set; } = new List<string>();

        public Header Header { get; set; }

        public List<TextSection> Sections { get; set; } = new List<TextSection>();

        public string Build(bool allowTooltips = true)
            => Locale.BuildMessage(this, allowTooltips);
    }
}