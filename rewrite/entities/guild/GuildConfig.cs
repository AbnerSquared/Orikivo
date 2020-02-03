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
        [Aliases("pfx")]
        [Subtitle("A value that will define the default prefix for all users in this server.")]
        public string Prefix { get; set; }
    }
}
