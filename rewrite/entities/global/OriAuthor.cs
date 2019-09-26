using Newtonsoft.Json;

namespace Orikivo
{
    public class OriAuthor
    {
        [JsonConstructor]
        internal OriAuthor(string name, ulong? id)
        {
            Name = name;
            Id = id;
        }

        public OriAuthor(OriUser oriUser)
        {
            Name = oriUser.DefaultName;
            Id = oriUser.Id;
        }

        public OriAuthor(string name)
        {
            Name = name;
        }

        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("id")]
        public ulong? Id { get; }
    }
}
