using Newtonsoft.Json;
using Orikivo.Static;

namespace Orikivo
{
    public class FontSheet
    {
        [JsonConstructor]
        public FontSheet(int index, string path)
        {
            Index = index;
            Index.Debug();
            Path = Locator.FontMaps + path;
            Path.Debug();
        }

        [JsonProperty("index")]
        public int Index { get; set; } // The array map box this font sheet references.

        [JsonProperty("path")]
        public string Path { get; set; } // The path of the file.
    }
}