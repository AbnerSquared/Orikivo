using Newtonsoft.Json;
using Orikivo.Desync;

namespace Orikivo
{
    /// <summary>
    /// Represents a custom message body that can be freely edited.
    /// </summary>
    public class MessageBuilder
    {
        public MessageBuilder() { }

        public MessageBuilder(string content, string url)
        {
            Content = content;
            Url = url;
            IsTTS = false;
        }

        [JsonConstructor]
        public MessageBuilder(string content, string url, Embedder embedder, bool isTts)
        {
            Content = content;
            Url = url;
            Embedder = embedder;
            IsTTS = isTts;
        }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("url")]
        public MessageUrl Url { get; set; }

        [JsonProperty("embedder")]
        public Embedder Embedder { get; set; }

        [JsonProperty("is_tts")]
        public bool IsTTS { get; set; }

        [JsonProperty("is_spoiler")]
        public bool IsSpoiler { get; set; }

        [JsonIgnore]
        public bool HasUrl => Url != null;

        [JsonIgnore]
        public bool CanEmbedUrl => Check.NotNull(Embedder) && Url?.Extension == ExtensionType.Image;

        public MessageBuilder WithEmbedder(Embedder embedder)
        {
            Embedder = embedder;
            return this;
        }

        public MessageBuilder WithContent(string content)
        {
            Content = content;
            return this;
        }

        public Message Build()
            => new Message(this);
    }
}
