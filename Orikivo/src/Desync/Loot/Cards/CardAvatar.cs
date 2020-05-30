namespace Orikivo.Desync
{
    // for Arcadia
    public class CardAvatar
    {
        public string DesignId { get; set; } // leave empty for default.
        public string Url { get; set; } // if Canvas is empty, default to Url, and then DefaultAvatarPath

        public bool CanAnimate { get; set; }
        public byte[] Canvas { get; set; }

        public int? LocalColorId { get; set; } // optional color scheme only for the avatar
        public CardAvatarScale Scale { get; set; }
        public int? Gamma { get; set; }
        public bool Active { get; set; }
    }
}
