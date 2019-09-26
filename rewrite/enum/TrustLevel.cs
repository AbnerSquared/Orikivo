using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Orikivo
{
    public enum TrustLevel
    {
        Inherit = 1, // inherit from the guild's config.
        Owner = 2, // limited to the for the owner of the guild.
        Dev = 3 // limited to the developer of the bot.
    }
}
