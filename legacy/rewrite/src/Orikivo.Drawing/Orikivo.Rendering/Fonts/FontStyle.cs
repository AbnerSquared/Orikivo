using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    public class FontStyle
    {
        [JsonConstructor]
        public FontStyle(FontType type, List<FontSheet> sheets)
        {
            Type = type;
            Type.Debug();
            Sheets = sheets;
            Sheets.Debug();
        }

        [JsonProperty("type")]
        public FontType Type { get; set; } // the style of font used.

        [JsonProperty("sheets")]
        public List<FontSheet> Sheets { get; set; } // The path of the font sheet.

        public bool TryGetSheet(int index, out FontSheet sheet)
        {
            sheet = null;
            if (!Sheets.Any(x => x.Index == index))
            {
                return false;
            }

            sheet = Sheets.Where(x => x.Index == index).First();
            return true;
        }
    }

    /// <summary>
    /// Represents a collection of font sheet data.
    /// </summary>
    public class FontSheetMap
    {
        [JsonConstructor]
        public FontSheetMap(FontSize size, List<FontSheet> sheets)
        {
            Size = size;
            Sheets = sheets;
        }

        [JsonProperty("size")]
        public FontSize Size { get; set; } // the font size for the sheets specified.

        [JsonProperty("sheets")]
        public List<FontSheet> Sheets { get; set; } // the collection of font sheets.
    }
}