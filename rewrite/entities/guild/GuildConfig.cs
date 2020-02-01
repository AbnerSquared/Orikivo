using Newtonsoft.Json;

namespace Orikivo.Unstable
{
    public class GuildConfig
    {
        public GuildConfig() { }

        [JsonConstructor]
        internal GuildConfig(string prefix)
        {
            Prefix = prefix;
        }

        [JsonProperty("prefix")]
        public string Prefix { get; set; }
    }
}
