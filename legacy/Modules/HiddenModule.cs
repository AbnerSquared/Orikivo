using Discord;
using Discord.Commands;
using Orikivo.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo.Modules
{
    // ensure this module is hidden from every single command.
    [Name("Hidden")]
    public class HiddenService : ModuleBase<OrikivoCommandContext>
    {
        [Command("isthis")]
        public async Task LossResponseAsync()
        {
            await ReplyAsync("\\| \\|\\| \\|\\| \\|_");
        }

        [Command("orica")]
        public async Task OricaResponseAsync()
        {
            EmbedBuilder eb = Embedder.DefaultEmbed;
            eb.WithDescription("**Orica** *(or·ick·ah)*\nAn orica is known to be the duration of time from when ||**REDACTED**|| occured to the events of ||**REDACTED**||. As Earth time, it is estimated to be around 122.2 years.");
            eb.WithColor(OriColor.Sunrise);

            await Context.Channel.SendEmbedAsync(eb.Build());
        }
    }
}
