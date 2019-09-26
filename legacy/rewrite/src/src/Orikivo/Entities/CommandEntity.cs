using Discord;
using Discord.Commands;

namespace Orikivo.Systems.Dependencies.Entities
{
    public static class CommandEntity
    {
        public static CommandServiceConfig GetDefault()
        {
            return new CommandServiceConfig
            {
                LogLevel = LogSeverity.Warning,
                DefaultRunMode = RunMode.Async,
                CaseSensitiveCommands = false
            };
        }

        public static CommandServiceConfig GetNew()
        {
            return new CommandServiceConfig();
        }
    }
}
