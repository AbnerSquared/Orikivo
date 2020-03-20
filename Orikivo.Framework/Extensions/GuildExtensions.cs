using Discord;
using System.Threading.Tasks;

namespace Orikivo
{
    public static class GuildExtensions
    {
        private static bool TryGetEmote(IGuild guild, string name, out GuildEmote emote)
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

        /// <summary>
        /// Attempts to get a specific emote from this <paramref name="guild"/>. If no matching emote could be found, this returns null.
        /// </summary>
        public static async Task<GuildEmote> GetEmoteAsync(this IGuild guild, string name, RequestOptions options = null)
        {
            if (TryGetEmote(guild, name, out GuildEmote emote))
                return await guild.GetEmoteAsync(emote.Id, options);

            return null;
        }
    }
}
