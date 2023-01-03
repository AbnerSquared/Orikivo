using Orikivo.Text;
using System.Collections.Generic;

namespace Arcadia
{

    // TODO: Include LocaleProvide for the ability to automatically parse localized string keys into their corresponding text
    public class TextBody
    {
        public Language Language { get; set; } = Language.English;

        public List<string> Tooltips { get; set; } = new List<string>();

        public string Warning { get; set; }

        public Header Header { get; set; }

        public List<TextSection> Sections { get; set; } = new List<TextSection>();

        public string Build(bool allowTooltips = true)
            => Locale.BuildMessage(this, allowTooltips);
    }
}
