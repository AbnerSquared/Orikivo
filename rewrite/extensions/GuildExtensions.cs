using Discord;

namespace Orikivo
{
    public static class GuildExtensions
    {
        public static bool TryGetEmote(this IGuild guild, string name, out GuildEmote emote)
        {
            emote = null;

            foreach (GuildEmote guildEmote in guild.Emotes)
            {
                if (guildEmote.Name.ToLower() == name.ToLower())
                {
                    emote = guildEmote;
                    return true;
                }
            }

            return false;
        }
    }
}
