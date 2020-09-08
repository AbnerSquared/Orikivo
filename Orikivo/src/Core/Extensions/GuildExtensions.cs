using System;
using System.Linq;
using Discord;

namespace Orikivo
{
    public static class GuildExtensions
    {
        /// <summary>
        /// Gets the specified emote in this guild.
        /// </summary>
        public static GuildEmote GetEmote(this IGuild guild, ulong id)
        {
            TryGetEmote(guild, id, out GuildEmote emote);
            return emote;
        }

        /// <summary>
        /// Gets the specified emote in this guild.
        /// </summary>
        public static GuildEmote GetEmote(this IGuild guild, string name)
        {
            TryGetEmote(guild, name, out GuildEmote emote);
            return emote;
        }

        /// <summary>
        /// Attempts to get the specified emote in this guild.
        /// </summary>
        public static bool TryGetEmote(this IGuild guild, ulong id, out GuildEmote emote)
        {
            emote = guild.Emotes.FirstOrDefault(x => x.Id == id);
            return emote != null;
        }

        /// <summary>
        /// Attempts to get the specified emote in this guild.
        /// </summary>
        public static bool TryGetEmote(this IGuild guild, string name, out GuildEmote emote)
        {
            emote = guild.Emotes.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return emote != null;
        }

    }
}
