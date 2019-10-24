using Newtonsoft.Json;

namespace Orikivo
{
    // TODO: Find the correct way to utilize this. As of now, all of this can be handled within Program.cs.
    /// <summary>
    /// A local entity used to store configurations for local processes.
    /// </summary>
    public class OriLocal
    {
        [JsonConstructor]
        internal OriLocal(DiscordConfig discordConfig, OriConsoleConfig consoleConfig, OriLogConfig loggerConfig)
        {
            DiscordConfig = discordConfig;
            ConsoleConfig = consoleConfig;
            LogConfig = loggerConfig;
        }

        [JsonProperty("discord_config")]
        public DiscordConfig DiscordConfig { get; }

        [JsonProperty("console_config")]
        public OriConsoleConfig ConsoleConfig { get; }

        [JsonProperty("log_config")]
        public OriLogConfig LogConfig { get; }
    }
}
