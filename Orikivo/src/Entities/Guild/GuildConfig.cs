using Newtonsoft.Json;

namespace Orikivo.Desync
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
        [Description("A value that will define the default prefix for all users in this server.")]
        public string Prefix { get; set; }
    }
}
