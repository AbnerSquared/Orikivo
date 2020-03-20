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

        public static GuildEmote GetEmote(this IGuild guild, string name)
        {
            TryGetEmote(guild, name, out GuildEmote emote);
            return emote;
        }

        public static GuildEmote GetEmote(this IGuild guild, ulong id)
        {
            TryGetEmote(guild, id, out GuildEmote emote);
            return emote;
        }

        public static bool TryGetEmote(this IGuild guild, ulong id, out GuildEmote emote)
        {
            emote = null;
            foreach (GuildEmote guildEmote in guild.Emotes)
            {
                if (guildEmote.Id == id)
                {
                    emote = guildEmote;
                    return true;
                }
            }

            return false;
        }
    }
}
