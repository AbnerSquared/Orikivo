using Newtonsoft.Json;

namespace Orikivo
{
    /// <summary>
    /// Represents a custom message body that can be freely edited.
    /// </summary>
    public class MessageBuilder
    {

        [JsonConstructor]
        internal MessageBuilder(string content, string url, Embedder embedder, bool isTTS)
        {
            Content = content;
            Url = url;
            Embedder = embedder;
            IsTTS = isTTS;
        }

        public MessageBuilder() { }

        public MessageBuilder(string content, string url)
        {
            Content = content;
            Url = url;
            IsTTS = false;
        }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("embedder")]
        public Embedder Embedder { get; set; }

        [JsonProperty("is_tts")]
        public bool IsTTS { get; set; } = false;

        [JsonProperty("is_spoiler")]
        public bool IsSpoiler { get; set; } = false;

        [JsonIgnore]
        public bool HasUrl => !string.IsNullOrWhiteSpace(Url);

        [JsonIgnore]
        public bool HideUrl { get; set; } = false;

        [JsonIgnore]
        public bool IsLocalUrl { get; set; } = false;

        [JsonIgnore]
        public bool CanEmbedUrl => Check.NotNull(Embedder) && EnumUtils.GetUrlExtension(Url).Value == ExtensionType.Image;

        [JsonIgnore]
        public ExtensionType? UrlExtension => HasUrl ? EnumUtils.GetUrlExtension(Url) : null;

        public Message Build()
            => new Message(this);
    }
}
