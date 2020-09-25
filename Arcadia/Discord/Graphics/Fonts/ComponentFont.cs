using Newtonsoft.Json;

namespace Arcadia.Graphics
{
    public class ComponentFont
    {
        [JsonConstructor]
        public ComponentFont(FontType font)
        {
            Font = font;
        }

        [JsonProperty("font")]
        public FontType Font { get; internal set; }
    }
}