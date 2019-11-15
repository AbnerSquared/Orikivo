using Newtonsoft.Json;

namespace Orikivo
{
    public class GuildCommand
    {
        [JsonConstructor]
        internal GuildCommand(string name, MessageBuilder message)
        {
            Name = name;
            Message = message;
            Author = null;
        }

        public GuildCommand(string name)
        {
            Catch.NotNull(name, nameof(name), "The name for a custom command cannot be empty.");
            Name = name;
            Author = null;
        }

        [JsonProperty("author")]
        public OriAuthor Author { get; }

        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("message")]
        public MessageBuilder Message { get; set; }
    }
}
