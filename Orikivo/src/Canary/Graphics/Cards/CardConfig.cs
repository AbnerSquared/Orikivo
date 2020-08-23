using Newtonsoft.Json;

namespace Orikivo.Canary
{
    /// <summary>
    /// Represents the configurations that can be applied to the design of a card.
    /// </summary>
    public class CardConfig
    {
        public CardConfig() { }

        [JsonConstructor]
        internal CardConfig(string nameFontId, string counterFontId, CardAvatarScale scale)
        {
            NameFontId = nameFontId;
            CounterFontId = counterFontId;
            AvatarScale = scale;
        }

        /// <summary>
        /// The <see cref="FontFace"/> that is used when drawing the card.
        /// </summary>
        [JsonProperty("name_font_id")]
        public string NameFontId { get; set; }

        [JsonProperty("counter_font_id")]
        public string CounterFontId { get; set; } // the font used when drawing counters

        [JsonProperty("avatar_scale")]
        public CardAvatarScale AvatarScale { get; set; }


    }
}
