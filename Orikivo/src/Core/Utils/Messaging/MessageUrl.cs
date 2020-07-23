using Newtonsoft.Json;

namespace Orikivo
{
    /// <summary>
    /// Represents a generic message URL.
    /// </summary>
    public class MessageUrl
    {
        public MessageUrl(string url)
        {
            Value = url;
        }

        [JsonConstructor]
        internal MessageUrl(string value, bool isLocal, bool isHidden)
        {
            Value = value;
            IsLocal = isLocal;
            IsHidden = isHidden;
        }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("local")]
        public bool IsLocal { get; set; }

        [JsonProperty("hidden")]
        public bool IsHidden { get; set; }

        [JsonIgnore]
        public ExtensionType? Extension
            => EnumUtils.GetUrlExtension(Value);

        [JsonIgnore]
        public bool IsEmbeddable
            => Extension == ExtensionType.Image;

        public override string ToString()
            => IsHidden ? Format.EscapeUrl(Value) : IsLocal ? EmbedUtils.CreateLocalImageUrl(Value) : Value;

        public static implicit operator string(MessageUrl url)
            => url.Value;

        public static implicit operator MessageUrl(string url)
            => new MessageUrl(url);
    }
}