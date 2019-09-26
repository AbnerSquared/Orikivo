using Discord.Commands;

namespace Orikivo
{
    [Name("Discord")]
    [Summary("Commands that provide information on Discord objects.")]
    public class DiscordModule : OriModuleBase<OriCommandContext>
    {
        // emojis global: gets all public emojis from all guilds using orikivo within the shard.
    }
}