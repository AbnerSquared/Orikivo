using System.Web;
using Discord;
using Discord.Net.Providers.WS4Net;
using Discord.WebSocket;

namespace Orikivo.Systems.Dependencies.Entities
{
    public static class SocketEntity
    {
        public static DiscordSocketConfig GetDefault()
        {
            return new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Warning,
                MessageCacheSize = 1000,
                AlwaysDownloadUsers = true
            };
        }

        public static DiscordSocketConfig GetNew()
        {
            return new DiscordSocketConfig();
        }
    }
}
