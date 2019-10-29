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
        }

        public GuildCommand(string name)
        {
            Catch.NotNull(name, nameof(name), "The name for a custom command cannot be empty.");
            Name = name;
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("message")]
        public MessageBuilder Message { get; set; }
    }
}
