using Discord.Commands;
using Orikivo;
using System.Threading.Tasks;

namespace Arcadia
{
    [Name("Casino")]
    [Summary("Come and gamble your life away.")]
    public class Casino : OriModuleBase<ArcadeContext>
    {
        [Command("gimi")]
        [RequireUser]
        [Summary("An activity that randomly offers a reward value (if you're lucky enough).")]
        public async Task GimiAsync()
        {
            var gimi = new Gimi();
            GimiResult result = gimi.Next();

            await Context.Channel.SendMessageAsync(result.ApplyAndDisplay(Context.Account));
        }
    }
}