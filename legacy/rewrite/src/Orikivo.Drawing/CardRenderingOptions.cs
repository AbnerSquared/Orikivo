using Newtonsoft.Json;

namespace Orikivo
{
    /// <summary>
    /// Represents a collection of options for a Card.
    /// </summary>
    public class CardRenderingOptions
    {
        [JsonProperty("font")]
        public FontFace Font { get; set; }

        [JsonProperty("colorscheme")]
        public ColorScheme Schema { get; set; }

        [JsonProperty("scale")]
        public int Scale { get; set; }

        //[JsonProperty("cardbackdrop")]
        //public CardBackdrop Backdrop { get; set; }

        [JsonProperty("cardborder")]
        public CardBorder Border { get; set; }

        [JsonProperty("showcasedmerit")]
        public OldMerit Merit { get; set; }

        // this is just if invalidated chars
        // fall back to a default font.
        public bool IgnoreInvalidChars { get; set; }

        [JsonProperty("casing")]
        public CaseValue Casing { get; set; }

        [JsonProperty("layout")]
        public CardLayoutOptions Layout { get; set; }
    }
}
