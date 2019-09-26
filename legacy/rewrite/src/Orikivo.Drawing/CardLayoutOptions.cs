using Newtonsoft.Json;

namespace Orikivo
{
    /// <summary>
    /// Represents the base layout configuration of a Card.
    /// </summary>
    public class CardLayoutOptions
    {
        [JsonProperty("avatardisplay")]
        public AvatarRenderFormat AvatarMode { get; set; }

        [JsonProperty("textdisplay")]
        public TextSize TextMode { get; set; }

        [JsonProperty("debtdisplay")]
        public DebtValue IncludeDebt { get; set; }

        [JsonProperty("moneydisplay")]
        public BalanceRenderType BalanceMode { get; set; }
    }
}
