using Discord.WebSocket;
using Newtonsoft.Json;

namespace Orikivo
{
    // Arcadia class
    public class GuildCommand
    {
        [JsonConstructor]
        internal GuildCommand(SocketUser user, string name, MessageBuilder message)
        {
            Author = new Author(user.Username, user.Id);
            Name = name;
            Message = message;
        }

        public GuildCommand(SocketUser user, string name)
        {
            Catch.NotNull(name, nameof(name), "The name for a custom command cannot be empty.");
            Author = new Author(user.Username, user.Id);
            Name = name;
            
        }

        [JsonProperty("author")]
        public Author Author { get; }

        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("message")]
        public MessageBuilder Message { get; set; }
    }
}
