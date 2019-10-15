using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    // used for storing local config
    public class OriLocal
    {
        [JsonConstructor]
        internal OriLocal(DiscordConfig discordConfig, OriConsoleConfig consoleConfig, OriLogConfig loggerConfig)
        {
            DiscordConfig = discordConfig;
            ConsoleConfig = consoleConfig;
            LoggerConfig = loggerConfig;
        }

        [JsonProperty("discord_config")]
        public DiscordConfig DiscordConfig { get; }
        [JsonProperty("console_config")]
        public OriConsoleConfig ConsoleConfig { get; }
        [JsonProperty("logger_config")]
        public OriLogConfig LoggerConfig { get; }
    }
}
